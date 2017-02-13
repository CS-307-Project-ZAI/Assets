using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public PlayerController player;
	public AllyController ally;
	public List<AllyController> people;
	public EnemyController enemy;
	public List<EnemyController> enemies;
	public List<Bullet> bullets;
	public int spawnAmount = 1;
	public Weapon startingWeapon;
	public string playerMode = "Combat";

	[HideInInspector]
	public List<PersonController> personKill;
	public List<Bullet> bulletKill;
	private bool paused = false;
	public UIController ui;
	private CameraController cam;

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
			}
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

		//Destroy each object in the kill list and clear it
		foreach (PersonController p in personKill) {
			if (p.gameObject.tag == "Enemy") {
				enemies.Remove ((EnemyController) p);
			} else if (p.gameObject.tag == "Ally") {
				people.Remove ((AllyController) p);
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
			ui.modeText.text = playerMode;
			//print (playerMode);
		}
		if (Input.GetKeyDown (KeyCode.Escape)) {
			paused = !paused;
		}
	}

	void spawnPlayer() {
		player = (PlayerController)Instantiate (player);
		player.gm = this;
	}

	void spawnAlly() {
		AllyController a = (AllyController)Instantiate (ally);
		people.Add (a);
		a.gm = this;
	}

	void spawnEnemy() {
		EnemyController e = (EnemyController) Instantiate (enemy);
		e.target = player;
		e.transform.position = new Vector3 (0, 0, 0);
		e.spawn = enemy;
		enemies.Add (e);
	}
}
