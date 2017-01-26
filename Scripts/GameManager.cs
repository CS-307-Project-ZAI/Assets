using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public PlayerController player;
	public List<EnemyController> enemies;
	public EnemyController enemy;
	public int spawnAmount = 1;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			for (int i = 0; i < spawnAmount; i++) {
				spawnEnemy ();
			}
		}
	}

	void spawnEnemy() {
		EnemyController e = (EnemyController) Instantiate (enemy);
		e.target = player;
		e.transform.position = new Vector3 (0, 0, 0);
		e.spawn = enemy;
		enemies.Add (e);
	}
}
