using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
	public bool devMode = false;
	public PlayerController player;
	public AllyController ally;
	public List<AllyController> people;
	public EnemyController enemy;
	public List<EnemyController> enemies;
	public List<EnemyController> targetedEnemies;
	public List<Bullet> bullets;
	public List<Building> buildings;
	public List<SpawnPylon> pylons;
	public int spawnAmount = 1;
	public string playerMode = "Combat";
	public PathFinding pf;
	public TriggersAttributes triggerAttribute;
	public Attributes Attribute;
	public AttributesZ AttributeZ;
	public LayerMask enemyMask;

	[HideInInspector]
	public bool paused = false;
	public UIController ui;
	public CameraController cam;
	public AllyController selectedAlly = null;
	public GameObject selectRing = null;
	public bool build = false;
	public bool buildDestroy = false;
	public bool recheckPaths = false;
	public bool setWaypoints = false;
    public int currentHour = 0;
	public int currentMinute = 0;
	public int currentDay = 0;
    public enum DayNightCycle {DAY, NIGHT};
    public DayNightCycle currentCycle = DayNightCycle.DAY;
    public int dayLength = 24;
	public int minutesPerHour = 60;
	public float minuteTimer = 0.0f;
	public float timeForMinute = 0.5f;
	public bool gameOver = false;

    string winDir = System.Environment.GetEnvironmentVariable("winDir");

	protected List<PersonController> personKill = new List<PersonController> ();
	protected List<Building> buildingKill = new List<Building> ();
	protected List<SpawnPylon> pylonKill = new List<SpawnPylon> ();
	protected List<Bullet> bulletKill = new List<Bullet> ();

	private int modeIndex = 0;
	private string[] modes = {"Combat", "Command", "Build"};
    private EnemySpawner spawner;

	private bool ready = false;
	// Use this for initialization
	void Start () {
		ui = FindObjectOfType<UIController> ();
		cam = FindObjectOfType<CameraController> ();
        spawner = new EnemySpawner(this);

		ui.GMStart ();
		if (!loadGameState()) {
			if (devMode) {
				Debug.Log ("Load failed...Starting new game!");
			}
			currentMinute = 0;
			currentHour = 12;
			currentDay = 1;
			spawnPlayer ();
			spawnAlly ();
		}

		cam.target = player;
		//ui.GMStart ();
		ready = true;
	}

	// Update is called once per frame
	void Update () {
		if (!ready) {
			return;
		}
		if (paused) {
			if (Input.GetKeyDown (KeyCode.Escape)) {
				paused = !paused;
				cam.SetCustomCursor ();
				foreach (AllyController a in people) {
					if (a.followingPath) {
						a.StartCoroutine ("FollowPath");
					}
				}
				foreach (EnemyController e in enemies) {
					if (e.followingPath) {
						e.StartCoroutine ("FollowPath");
					}
				}
			} else {
				foreach (AllyController a in people) {
					a.StopCoroutine ("FollowPath");
				}
				foreach (EnemyController e in enemies) {
					e.StopCoroutine ("FollowPath");
				}
			}
			ui.GMUpdate ();
			return;
		}
        updateDayNightCycle();

		spawner.checkSpawnTime();

		//Get Game Actions
		getManagerActions ();

		//Update UI
		ui.GMUpdate();

		//Update Player
		player.GMUpdate ();

		//Update Camera
		cam.GMUpdate();

		//Update Bullets
		foreach (Bullet b in bullets) {
			b.GMUpdate ();
			if (b.kill) {
				bulletKill.Add (b);
			}
		}

		//Update Enemies
		foreach (EnemyController e in enemies) {
			if (getEuclideanDistance (player, e) < 20) {
				if (e.interrupted) {
					e.interrupted = false;
					e.StartCoroutine ("FollowPath");
				}
				e.GMUpdate ();
				if (e.kill) {
					personKill.Add (e);
				}
			} else if (e.followingPath && !e.interrupted) {
				e.StopCoroutine ("FollowPath");
				e.interrupted = true;
			}
		}

		//Update Allies
		foreach (AllyController a in people) {
			a.GMUpdate ();
			if (a.kill) {
				personKill.Add (a);
			}
			recheckPaths = false;
		}

		//Updated selectedAlly
		if (selectedAlly != null) {
			if (selectRing == null) {
				string load = "Other/SelectRing";
				selectRing = (GameObject) Instantiate(Resources.Load (load, typeof(GameObject)) as GameObject);
				foreach (GameObject obj in selectedAlly.waypoints) {
					obj.SetActive (true);
				}
			}
			selectRing.transform.position = new Vector3 (selectedAlly.transform.position.x, selectedAlly.transform.position.y, 0);
		}

		//Update Buildings
		foreach (Building b in buildings) {
			b.GMUpdate ();
			if (b.kill) {
				buildingKill.Add (b);
			}
		}

		//Update Spawn Pylons
		foreach (SpawnPylon p in pylons) {
			p.GMUpdate ();
			if (p.kill) {
				pylonKill.Add (p);
			}
		}  

		//Destroy each object in the kill list and clear it
		foreach (PersonController p in personKill) {
			PathRequestManager.RemoveRequest (p);
			if (p.gameObject.tag == "Enemy") {
                enemies.Remove ((EnemyController) p);
				if (targetedEnemies.IndexOf ((EnemyController) p) >= 0) {
					targetedEnemies.Remove ((EnemyController) p);
				}
			} else if (p.gameObject.tag == "Ally") {
				people.Remove ((AllyController) p);
				if (p == selectedAlly) {
					selectedAlly = null;
					Destroy (selectRing);
					selectRing = null;
				}
			}
			foreach (PersonController other in p.othersInfluenced) {
				if (other.gameObject.tag == "Enemy") {
					EnemyController temp = (EnemyController)other;
					temp.stats.proximityAllies.Remove (p);
					temp.stats.proximityEnemies.Remove (p);
					temp.stats.proximityNPCs.Remove (p);
				} else if (other.gameObject.tag == "Ally") {
					AllyController temp = (AllyController)other;
					temp.stats.proximityAllies.Remove (p);
					temp.stats.proximityEnemies.Remove (p);
					temp.stats.proximityNPCs.Remove (p);
				}
			}
			p.StopCoroutine ("FollowPath");
			Destroy (p.gameObject);
		}
		personKill.Clear ();

		//Destroy buildings in kill list and clear it
		foreach (Building b in buildingKill) {
			buildings.Remove (b);
			Destroy (b.gameObject);
		}
		buildingKill.Clear ();

		//Destroy pylons in kill list and clear it
		foreach (SpawnPylon p in pylonKill) {
			pylons.Remove (p);
			Destroy (p.gameObject);
		}
		pylonKill.Clear ();

		//Destroy bullets in kill list and clear it
		foreach (Bullet b in bulletKill) {
			bullets.Remove (b);
			Destroy (b.gameObject);
		}
		bulletKill.Clear ();
    }

	void getManagerActions() {
		if (Input.GetKeyDown (KeyCode.J)) {
			for (int i = 0; i < spawnAmount; i++) {
				spawnEnemy ();
			}
		}
		if (Input.GetKeyDown (KeyCode.Space)) {
			modeIndex = (modeIndex + 1) % modes.Length;
			playerMode = modes[modeIndex];
			selectedAlly = null;
			Destroy (selectRing);
			selectRing = null;
		}
		if (Input.GetKeyDown (KeyCode.Escape)) {
			paused = !paused;
			cam.SetCustomCursor ();
		}
	}

	public float getEuclideanDistance(PersonController a, PersonController b) {
		return Mathf.Sqrt (Mathf.Pow (a.transform.position.x - b.transform.position.x, 2) + Mathf.Pow (a.transform.position.y - b.transform.position.y, 2));
	}

	public GameObject getClickedObject(int option) {
		//Converting Mouse Pos to 2D (vector2) World Pos
		Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
		RaycastHit2D hit;
		if (option == 1) {
			hit = Physics2D.Raycast (rayPos, Vector2.zero, 0f, pf.grid.unwalkableMask);
		} else if (option == 2) {
			hit = Physics2D.Raycast (rayPos, Vector2.zero, 0f, enemyMask); 
		} else {
			hit = Physics2D.Raycast (rayPos, Vector2.zero, 0f);
		}
		if (hit) {
			if (devMode) {
				Debug.Log (hit.transform.name);
			}
			return hit.transform.gameObject;
		}
		else return null;
	}

	void spawnPlayer() {
		player = (PlayerController)Instantiate (player);
		player.gm = this;
		player.personName = "Player";

		player.addWeapon ("Pistol");
		//player.addWeapon ("Revolver");
		//player.addWeapon ("Shotgun");
		//player.addWeapon ("Machine Gun");
	}

	void spawnAlly() {
		AllyController a = (AllyController)Instantiate (ally);
		people.Add (a);
		a.gm = this;
		a.personName = "Billy";
		a.transform.position = new Vector3 (2, 0, 0);

		a.addWeapon ("Pistol");
	}
    
	public void spawnEnemyAtLocation(Vector3 spawnLocation, int spawnID) {
		EnemyController e = (EnemyController) Instantiate (enemy);
		e.transform.position = spawnLocation;
       	e.gm = this;
        e.spawnID = spawnID;
		enemies.Add (e);
	}
    
    public void spawnEnemy() {
		spawnEnemyAtLocation(Vector3.zero, 0);
    }

	public void createSpawnPylon(Vector3 pos) {

	}

	public void targetEnemy(EnemyController e) {
		if (targetedEnemies.IndexOf (e) >= 0) {
			targetedEnemies.Remove (e);
			if (e.targetTag != null) {
				Destroy (e.targetTag);
				e.targetTag = null;
			}
		} else {
			targetedEnemies.Add (e);
			if (e.targetTag != null) {
				Destroy (e.targetTag);
			}
			e.targetTag = (GameObject) Instantiate (Resources.Load ("Other/TargetTag", typeof(GameObject)) as GameObject);
			e.targetTag.transform.parent = e.transform;
			e.targetTag.transform.position = new Vector3 (e.transform.position.x, e.transform.position.y + 0.5f, 0);
		}
	}

	public void deselectAlly() {
		if (selectedAlly != null) {
			foreach (GameObject obj in selectedAlly.waypoints) {
				obj.SetActive (false);
			}
			selectedAlly = null;
			if (selectRing != null) {
				Destroy (selectRing);
			}
			selectRing = null;
		}
	}

	public void toggleBuild(bool objectBuilt) {
		if (objectBuilt) {
			recreateGrid();
		}
		this.build = !this.build;
		this.buildDestroy = false;
		ui.pd.collisions.Clear ();
		cam.SetCustomCursor ();
	}

	public void toggleBuildDestroy() {
		this.buildDestroy = !this.buildDestroy;
		this.build = false;
		cam.SetCustomCursor ();
	}

	public void toggleWaypoints() {
		this.setWaypoints = !setWaypoints;
		cam.SetCustomCursor ();
	}

	public void deleteLastWaypoint() {
		if (selectedAlly != null) {
			selectedAlly.removeLastWaypoint ();
		}
	}

	public void recreateGrid() {
		recheckPaths = true;
		pf.grid.CreateGrid ();
	}

	private void updateDayNightCycle() {
		minuteTimer += Time.deltaTime;
		if (minuteTimer >= timeForMinute) {
			minuteTimer = 0.0f;
			currentMinute += 1;
			if (currentMinute >= minutesPerHour) {
				currentMinute = 0;
				currentHour += 1;
				if (currentHour >= dayLength) {
					currentHour = 0;
					currentDay += 1;
					ui.updateDay (currentDay);
				}
				if (currentHour < dayLength / 2) {
					currentCycle = DayNightCycle.DAY;
				} else {
					currentCycle = DayNightCycle.NIGHT;
				}
				ui.updateHour (currentHour);
			}
			ui.updateMinute (currentMinute);
		}
	}

	public void endGame() {
		gameOver = true;
		if (ApplicationModel.savefile != 0) {
			string path = ApplicationModel.savePath + "savefile" + ApplicationModel.savefile + ".txt";
			File.Delete (path);
		}
	}

	public void backToMenu() {
		SceneManager.LoadSceneAsync ("Scenes/Main Menu");
	}

	public void quitToMenu(int option) {
		if (ApplicationModel.savefile != 0 && option == 1) {
			createSave ();
		}
		SceneManager.LoadSceneAsync ("Scenes/Main Menu");
	}

	public bool createSave() {
		if (ApplicationModel.savefile == 0) {
			ApplicationModel.savefile = ApplicationModel.getEmptySaveFile ();
			if (ApplicationModel.savefile == -1) {
				return false;
			}
		}

		//Open C# file writer
		StreamWriter writer = new StreamWriter (ApplicationModel.savePath + "savefile" + ApplicationModel.savefile + ".txt");

		//Player Position and health
		writer.WriteLine((string)(player.transform.position.x + "," + player.transform.position.y + "," + player.health + "," + 
			player.playerInventory["cloth"] + "," + player.playerInventory["wood"] + "," + player.playerInventory["metal"]));

		//Player's Weapons
		writer.WriteLine(player.weapons.Count.ToString());
		foreach (Weapon w in player.weapons) {
			writer.WriteLine((string)(w.weaponName + "," + w.currentLoaded + "," + w.ammoPool));
		}
			
		//Ally positions and health
		writer.WriteLine(people.Count.ToString());
		foreach (AllyController a in people) {
			writer.WriteLine((string)(a.personName + "," + a.transform.position.x + "," + a.transform.position.y + "," + a.health));
		}

		//Enemy targets, positions, and health
		writer.WriteLine(enemies.Count.ToString());
		foreach (EnemyController e in enemies) {
			if (e.target == null) {
				writer.WriteLine ((string)("," + e.transform.position.x + "," + e.transform.position.y + "," + e.health));
			} else {
				writer.WriteLine ((string)(e.target.personName + "," + e.transform.position.x + "," + e.transform.position.y + "," + e.health));
			}
		}

		//Building positions and rotations
		writer.WriteLine(buildings.Count.ToString());
		foreach (Building b in buildings) {
			writer.WriteLine((string)(b.buildingName + "," + b.transform.position.x + "," + b.transform.position.y + "," + b.transform.eulerAngles.z + "," + b.buildingHealth));
		}

		//Close the writer
		if (devMode) {
			Debug.Log ("File saved to " + ApplicationModel.savePath + "savefile" + ApplicationModel.savefile + ".txt");
		}
		writer.Close ();
		return true;
	}

	public bool loadGameState() {
		if (ApplicationModel.savefile == 0) {
			return false;
		}

		StreamReader reader = new StreamReader (ApplicationModel.savePath + "savefile" + ApplicationModel.savefile + ".txt", Encoding.Default);

		using (reader) {
			//Load Player
			string[] playerInfo = reader.ReadLine ().Split(',');
			if (!loadPlayer (playerInfo)) {
				return false;
			}

			//Load Player Weapons
			int numWeapons = int.Parse (reader.ReadLine());
			string[] weaponInfo;
			for (int i = 0; i < numWeapons; i++) {
				weaponInfo = reader.ReadLine ().Split (',');
				if (!loadWeapon (weaponInfo)) {
					return false;
				}
			}

			//Load Allies
			int numAllies = int.Parse (reader.ReadLine());
			string[] allyInfo;
			for (int i = 0; i < numAllies; i++) {
				allyInfo = reader.ReadLine ().Split (',');
				if (!loadAlly(allyInfo)) {
					return false;
				}
			}

			//Load Enemies
			int numEnemies = int.Parse (reader.ReadLine());
			string[] enemyInfo;
			for (int i = 0; i < numEnemies; i++) {
				enemyInfo = reader.ReadLine ().Split (',');
				if (!loadEnemy (enemyInfo)) {
					return false;
				}
			}

			//Load Buildings
			int numBuildings = int.Parse (reader.ReadLine());
			string[] buildingInfo;
			for (int i = 0; i < numBuildings; i++) {
				buildingInfo = reader.ReadLine ().Split (',');
				if (!loadBuilding (buildingInfo)) {
					return false;
				}
			}

			reader.Close ();
		}
		return true;
	}

	public bool loadPlayer(string[] info) {
		if (info.Length < 6) {
			return false;
		}
		player = (PlayerController)Instantiate (player);
		player.gm = this;
		player.personName = "Player";
		player.transform.position = new Vector3 (float.Parse(info[0]), float.Parse(info[1]), 0);
		player.health = int.Parse(info [2]);
		player.playerInventory = new Dictionary<string, int>();
		player.playerInventory.Add ("cloth", 0);
		player.playerInventory.Add ("wood", 0);
		player.playerInventory.Add ("metal", 0);
		player.playerInventory ["cloth"] = int.Parse (info [3]);
		player.playerInventory["wood"] = int.Parse (info [4]);
		player.playerInventory["metal"] = int.Parse (info [5]);
		return true;
	}

	public bool loadWeapon(string[] info) {
		if (info.Length < 3) {
			return false;
		}
		Weapon w = player.addWeapon (info [0]);
		w.currentLoaded = int.Parse (info [1]);
		w.ammoPool = int.Parse (info [2]);
		return true;
	}

	public bool loadAlly(string[] info) {
		if (info.Length < 4) {
			return false;
		}

		AllyController a = (AllyController)Instantiate (ally);
		people.Add (a);
		a.gm = this;
		a.personName = info[0];
		a.transform.position = new Vector3 (float.Parse(info[1]), float.Parse(info[2]), 0);
		a.health = int.Parse (info [3]);

		//Give person starting weapon
		a.addWeapon("Pistol");
		return true;
	}

	public bool loadEnemy(string[] info) {
		if (info.Length < 4) {
			return false;
		}
		EnemyController e = (EnemyController) Instantiate (enemy);
		e.transform.position = new Vector3(float.Parse(info[1]), float.Parse(info[2]), 0);
		e.gm = this;
		e.spawnID = 0;
		if (info[0] == "Player") {
			e.target = player;
		}
		e.health = int.Parse (info [3]);
		enemies.Add (e);
		return true;
	}

	public bool loadBuilding(string[] info) {
		if (info.Length < 5) {
			return false;
		}
		string loadBuilding = "Buildings/" + info[0];
		Building newBuilding = Resources.Load(loadBuilding, typeof(Building)) as Building;
		newBuilding = Instantiate (newBuilding) as Building;
		newBuilding.transform.position = new Vector3 (float.Parse(info[1]), float.Parse(info[2]), 0);
		newBuilding.transform.Rotate (new Vector3 (0, 0, float.Parse (info [3])));
		newBuilding.gm = this;
		newBuilding.buildingHealth = int.Parse (info[4]);
		buildings.Add (newBuilding);
		return true;
	}
}
