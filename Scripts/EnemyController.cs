using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : PersonController {

	public PlayerController target;
	public float spawnRate = 5.0f;
	public float attackRate = 1.0f;
	public int spawnAmount = 1;
	public int damage = 2;
	public EnemyController spawn;

	float spawnTimer = 0.0f;

	// Use this for initialization
	new void Start () {
		spawnTimer = 0.0f;
		attackTimer = 0.0f;
		gm = FindObjectOfType<GameManager> ();
	}
	
	// Update is called once per frame
	public void GMUpdate () {
		attackTimer += Time.deltaTime;
		getMovement ();
		getRotation ();
		checkSpawnTime ();
	}

	void getMovement() {
		float dirX = target.transform.position.x - transform.position.x;
		float dirY = target.transform.position.y - transform.position.y;
		Vector3 dir = new Vector3(dirX, dirY, 0);
		transform.position += Vector3.ClampMagnitude(dir, moveSpeed) * Time.deltaTime;
	}

	void getRotation() {
		transform.rotation = Quaternion.LookRotation (Vector3.forward, target.transform.position - transform.position);
		transform.Rotate (new Vector3 (0, 0, 45.0f));
	}

	void checkSpawnTime() {
		spawnTimer += Time.deltaTime;
		if (spawnTimer > spawnRate) {
			for (int i = 0; i < spawnAmount; i++) {
				spawnChild ();
			}
			spawnTimer = 0.0f;
		}
	}

	void spawnChild() {
		EnemyController e = (EnemyController) Instantiate (spawn);
		e.target = target;
		e.transform.position = transform.position + new Vector3(1, 0, 0);
		e.spawn = spawn;
		gm.enemies.Add (e);
	}

	void OnTriggerStay2D(Collider2D col) {
		if (col.gameObject.tag == "Player") {
			if (attackTimer >= attackRate) {
				col.gameObject.SendMessage ("ApplyDamage", damage);
				attackTimer = 0.0f;
			}
		} else if (col.gameObject.tag == "Ally") {
			if (attackTimer >= attackRate) {
				col.gameObject.SendMessage ("ApplyDamage", damage);
				attackTimer = 0.0f;
			}
		}
	}
}
