using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public PlayerController player;
	public AllyController ally;
	public List<AllyController> people;
	public EnemyController enemy;
	public List<EnemyController> enemies;
	public List<EnemyController> targetedEnemies;
	public List<Bullet> bullets;
	public List<Wall> walls;
	public int spawnAmount = 1;
	public Weapon startingWeapon;
	public string playerMode = "Combat";
	public string difficulty = "Easy";
	public PathFinding pf;
	public TriggersAttributes triggerAttribute;

	[HideInInspector]
	public List<PersonController> personKill;
	public List<Bullet> bulletKill;
	public bool paused = false;
	public UIController ui;
	public CameraController cam;
	public AllyController selectedAlly = null;
	public GameObject selectRing = null;
	public bool build = false;
	public bool buildDestroy = false;
	public bool recheckPaths = false;
	public bool setWaypoints = false;

    public int currentTimeOfDay = 0;
    public enum DayNightCycle {DAY, NIGHT};
    public DayNightCycle currentCycle = DayNightCycle.DAY;
    public int dayLength = 60;

    string winDir = System.Environment.GetEnvironmentVariable("winDir");

	private int modeIndex = 0;
	private string[] modes = {"Combat", "Command", "Build"};
    private EnemySpawner spawner;

	// Use this for initialization
	void Start () {
		ui = FindObjectOfType<UIController> ();
		cam = FindObjectOfType<CameraController> ();
		ui.GMStart ();
        spawner = new EnemySpawner(this);
		spawnPlayer ();
		spawnAlly ();
	}

	// Update is called once per frame
	void Update () {
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

		//Get Game Actions
		getManagerActions ();


		//Update UI
		ui.GMUpdate();

		//Update Camera
		cam.GMUpdate();

		//Update Player
		player.GMUpdate ();

		//Update Enemies
		foreach (EnemyController e in enemies) {
			e.GMUpdate ();
			if (e.kill) {
				personKill.Add (e);
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
			Destroy (p.gameObject);
		}
		personKill.Clear ();

		//Update Bullets
		foreach (Bullet b in bullets) {
			b.GMUpdate ();
			if (b.kill) {
				bulletKill.Add (b);
			}
		}
		foreach (Bullet b in bulletKill) {
			bullets.Remove (b);
			Destroy (b.gameObject);
		}
		bulletKill.Clear ();

        spawner.checkSpawnTime();
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

	public GameObject getClickedObject() {
		//Converting Mouse Pos to 2D (vector2) World Pos
		Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
		RaycastHit2D hit=Physics2D.Raycast(rayPos, Vector2.zero, 0f);
		if (hit) {
			Debug.Log (hit.transform.name);
			return hit.transform.gameObject;
		}
		else return null;
	}

	void spawnPlayer() {
		player = (PlayerController)Instantiate (player);
		player.gm = this;
		player.personName = "Player";
	}

	void spawnAlly() {
		AllyController a = (AllyController)Instantiate (ally);
		people.Add (a);
		a.gm = this;
		a.personName = "Billy";
		a.transform.position = new Vector3 (2, 0, 0);
	}
    
	public void spawnEnemyAtLocation(Vector3 spawnLocation) {
		EnemyController e = (EnemyController) Instantiate (enemy);
		e.target = player;
		e.transform.position = spawnLocation;
        e.gm = this;
		enemies.Add (e);
	}

    
    public void spawnEnemy() {
        spawnEnemyAtLocation(Vector3.zero);
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

	public void createSave() {
		//Open C# file writer
		StreamWriter writer = new StreamWriter ("savefile.txt");

		//Player Position and health
		writer.WriteLine((string)(player.transform.position.x + "," + player.transform.position.y + "," + player.health + "\n"));

		//Player's Weapons
		writer.WriteLine((string)(player.weapons.Count + "\n"));
		foreach (Weapon w in player.weapons) {
			writer.WriteLine((string)(w.weaponName + "," + w.currentLoaded + "," + w.ammoPool + "\n"));
		}
			
		//Ally positions and health
		writer.WriteLine((string)(people.Count + "\n"));
		foreach (AllyController a in people) {
			writer.WriteLine((string)(a.personName + "," + a.transform.position.x + "," + a.transform.position.y + "," + a.health + "\n"));
		}

		//Enemy targets, positions, and health
		writer.WriteLine((string)(enemies.Count + "\n"));
		foreach (EnemyController e in enemies) {
			writer.WriteLine((string)(e.target.personName + "," + e.transform.position.x + "," + e.transform.position.y + "," + e.health + "\n"));
		}

		//Wall positions and rotations
		writer.WriteLine((string)(walls.Count + "\n"));
		foreach (Wall w in walls) {
			writer.WriteLine((string)(w.wallTier + "," + w.transform.position.x + "," + w.transform.position.y + "," + w.transform.eulerAngles.z + "," + w.wallHealth + "\n"));
		}

		//Close the writer
		Debug.Log("File saved to /ProjectZAI/savefile.txt");
		writer.Close ();
	}

    private void updateDayNightCycle() {
        currentTimeOfDay = (int)Time.realtimeSinceStartup % dayLength;
        if (currentTimeOfDay < dayLength / 2)
        {
            currentCycle = DayNightCycle.DAY;
        }
        else {
            currentCycle = DayNightCycle.NIGHT;
        }

    }

}
