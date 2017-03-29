using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner{
    public GameManager gm;
    public float maxDistanceToSpawnFromPlayer = 5.0f;
    public float minDistanceToSpawnFromPlayer = 2.0f;
    public float minDistanceToSpawnFromOther = 1.0f;
    public float spawnTimer = 0.0f;
    public float spawnRate = 5.0f;
    public int spawnAmount = 1;

    public EnemySpawner(GameManager g) {
        gm = g;
    }

    public void checkSpawnTime()
    {
        float currentSpawnRate = spawnRate;
        if (gm.currentCycle == GameManager.DayNightCycle.NIGHT) currentSpawnRate = spawnRate * 2;
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= currentSpawnRate)
        {
            for (int i = 0; i < spawnAmount; i++)
            {
                spawnChild();
            }
            spawnTimer = 0.0f;
        }
    }

    bool isValidSpawn(Vector3 spawnPos) {
        if ((gm.player.transform.position - spawnPos).magnitude < minDistanceToSpawnFromPlayer) return false;
        foreach (EnemyController e in gm.enemies) {
            float dist = (e.transform.position - spawnPos).magnitude;
            if (dist < minDistanceToSpawnFromOther) return false;
        }
        foreach (AllyController e in gm.people)
        {
            float dist = (e.transform.position - spawnPos).magnitude;
            if (dist < minDistanceToSpawnFromOther) return false;
        }
        foreach (Wall e in gm.walls)
        {
            float dist = (e.transform.position - spawnPos).magnitude;
            if (dist < minDistanceToSpawnFromOther) return false;
        }
        return true;
    }

    Vector3 getRandomSpawn()
    {
        return gm.player.transform.position + new Vector3(Random.Range(-maxDistanceToSpawnFromPlayer, maxDistanceToSpawnFromPlayer), Random.Range(-maxDistanceToSpawnFromPlayer, maxDistanceToSpawnFromPlayer), 0);
    }

    void spawnChild()
    {
		Vector3 spawnPos;
		do {
			spawnPos = getRandomSpawn ();
		} while (!isValidSpawn (spawnPos));
        gm.spawnEnemyAtLocation(spawnPos);
    }
}
