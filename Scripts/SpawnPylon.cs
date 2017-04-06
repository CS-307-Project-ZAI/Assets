using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPylon : MonoBehaviour {

	public float spawnRate;
	public float spawnTimer = 0.0f;
	public int spawnAmount = 1;
	public int health = 500;
	public bool kill = false;
	public GameManager gm;
	
	// Update is called once per frame
	public void GMUpdate () {
		float currentSpawnRate = spawnRate;
		if (gm.currentCycle == GameManager.DayNightCycle.NIGHT) currentSpawnRate = spawnRate * 2;
		spawnTimer += Time.deltaTime;
		if (spawnTimer >= currentSpawnRate)
		{
			for (int i = 0; i < spawnAmount; i++)
			{
				gm.spawnEnemyAtLocation(this.transform.position, 0);
			}
			spawnTimer = 0.0f;
		}
	}

	void ApplyDamage(int dmg) {
		this.health -= dmg;
		aliveCheck ();
	}

	public bool aliveCheck() {
		if (health <= 0) {
			kill = true;
			return false;
		}
		return true;
	}
}
