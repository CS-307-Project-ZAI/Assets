using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public PlayerController player;
	public AllyController ally;
	public List<AllyController> people;
	public EnemyController enemy;
	public List<EnemyController> enemies;
	public int spawnAmount = 1;
	public Weapon startingWeapon;

	// Use this for initialization
	void Start () {
		spawnPlayer ();
		spawnAlly ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			for (int i = 0; i < spawnAmount; i++) {
				spawnEnemy ();
			}
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
