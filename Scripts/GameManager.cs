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
	public int spawnAmount = 1;
	public Weapon startingWeapon;
	public string playerMode = "Combat";

	[HideInInspector]
	public List<PersonController> personKill;
	public List<Bullet> bulletKill;
	public bool paused = false;
	public UIController ui;
	public CameraController cam;
	public AllyController selectedAlly = null;
	public GameObject selectRing = null;

	private int modeIndex = 0;
	private string[] modes = {"Combat", "Command", "Build"};

	// Use this for initialization
	void Start () {
		ui = FindObjectOfType<UIController> ();
		cam = FindObjectOfType<CameraController> ();
		ui.GMStart ();
		spawnPlayer ();
		spawnAlly ();
	}
	
	// Update is called once per frame
	void Update () {
		if (paused) {
			if (Input.GetKeyDown (KeyCode.Escape)) {
				paused = !paused;
				cam.SetCustomCursor ();
			}
			ui.GMUpdate ();
			return;
		}
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
		}

		//Updated selectedAlly
		if (selectedAlly != null) {
			if (selectRing == null) {
				string load = "Other/SelectRing";
				selectRing = (GameObject) Instantiate(Resources.Load (load, typeof(GameObject)) as GameObject);
			}
			selectRing.transform.position = new Vector3 (selectedAlly.transform.position.x, selectedAlly.transform.position.y, 0);
		}

		//Destroy each object in the kill list and clear it
		foreach (PersonController p in personKill) {
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
	}

	void spawnAlly() {
		AllyController a = (AllyController)Instantiate (ally);
		people.Add (a);
		a.gm = this;
		a.personName = "Billy";
		a.transform.position = new Vector3 (2, 0, 0);
	}

	void spawnEnemy() {
		EnemyController e = (EnemyController) Instantiate (enemy);
		e.target = player;
		e.transform.position = new Vector3 (0, 0, 0);
		e.spawn = enemy;
		enemies.Add (e);
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
}
