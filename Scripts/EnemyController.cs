using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

	public int health = 50;
	public float moveSpeed = 0.5f;
	public PlayerController target;
	public float spawnTime = 5.0f;
	public int spawnAmount = 1;
	public EnemyController spawn;

	float timer = 0.0f;

	// Use this for initialization
	void Start () {
		timer = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
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
		timer += Time.deltaTime;
		if (timer > spawnTime) {
			for (int i = 0; i < spawnAmount; i++) {
				spawnChild ();
			}
			timer = 0.0f;
		}
	}

	void spawnChild() {
		EnemyController e = (EnemyController) Instantiate (spawn);
		e.target = target;
		e.transform.position = transform.position + new Vector3(1, 0, 0);
		e.spawn = spawn;
		e.timer = 0.0f;
	}

	public void aliveCheck() {
		if (health <= 0) {
			Destroy (gameObject);
		}
	}

	void ApplyDamage(int dmg) {
		this.health -= dmg;
		aliveCheck ();
	}
}
