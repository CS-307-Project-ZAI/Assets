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
	public List<EnemyController> enemyKill;
	public List<Bullet> bulletKill;

	private int modeIndex = 0;
	private string[] modes = {"Combat", "Command", "Build"};

	// Use this for initialization
	void Start () {
		spawnPlayer ();
		spawnAlly ();
	}
	
	// Update is called once per frame
	void Update () {
		//Get Game Actions
		getManagerActions ();

		//Update Player
		player.GMUpdate ();

		//Update Enemies
		foreach (EnemyController e in enemies) {
			e.GMUpdate ();
			if (e.kill) {
				enemyKill.Add (e);
			}
		}
		foreach (EnemyController e in enemyKill) {
			enemies.Remove (e);
			Destroy (e.gameObject);
		}
		enemyKill.Clear ();

		//Update Allies
		foreach (AllyController a in people) {
			a.GMUpdate ();
		}

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
			print (playerMode);
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
