using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuManager : GameManager {

	public AllyController dummyAlly;
	bool settingsActive = false;

	public float spawnTimer = 0.0f;
	public float spawnTime = 3.0f;

	public RectTransform mainMenu;
	public RectTransform settingsMenu;
	Slider volumeSlider;

	// Use this for initialization
	void Start () {
		mainMenu.gameObject.SetActive (true);
		settingsMenu.gameObject.SetActive (false);
		volumeSlider = settingsMenu.Find ("Volume Slider").GetComponent<Slider>();

		player = (PlayerController)Instantiate (player);
		player.gm = this;
		player.personName = "DummyPlayer";
		personKill.Add (player);

		dummyAlly = (AllyController)Instantiate (ally);
		people.Add (dummyAlly);
		dummyAlly.gm = this;
		dummyAlly.personName = "DummyBilly";
		dummyAlly.transform.position = new Vector3 (-6.25f, -1.75f, 0);
		dummyAlly.addWaypoint (new Vector3 (-4, 2, 0));
		dummyAlly.addWaypoint (new Vector3 (-4, -1.5f, 0));
		dummyAlly.addWaypoint (new Vector3 (-8, 0, 0));
		dummyAlly.mode = "Points";
		dummyAlly.rotationFix = 45.0f;
		foreach (GameObject obj in dummyAlly.waypoints) {
			obj.SetActive (false);
		}
		dummyAlly.health = 999999999;
	}
	
	// Update is called once per frame
	void Update () {
		//Update Enemies
		foreach (EnemyController e in enemies) {
			e.GMUpdate ();
			if (e.kill) {
				personKill.Add (e);
			}
		}

		//Update Ally
		if (dummyAlly != null) {
			dummyAlly.GMUpdate ();
			if (dummyAlly.kill) {
				personKill.Add (dummyAlly);
			}
		}

		//Destroy each object in the kill list and clear it
		foreach (PersonController p in personKill) {
			PathRequestManager.RemoveRequest (p);
			if (p.gameObject.tag == "Enemy") {
				enemies.Remove ((EnemyController) p);
			} else if (p.gameObject.tag == "Ally") {
				dummyAlly = null;
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

		spawnTimer += Time.deltaTime;
		if (spawnTimer >= spawnTime) {
			spawnTimer = 0.0f;

			spawnEnemyAtLocation (new Vector3 (Random.Range(15.0f, 20.0f), Random.Range(-5.0f, 5.0f), 0));
		}
	}

	public void spawnEnemyAtLocation(Vector3 spawnLocation) {
		EnemyController e = (EnemyController) Instantiate (enemy);
		e.target = dummyAlly;
		e.transform.position = spawnLocation;
		e.gm = this;
		enemies.Add (e);
	}

	public void startNewGame() {
		SceneManager.LoadSceneAsync ("Scenes/test1");
	}

	public void toggleSettings() {
		settingsActive = !settingsActive;
		if (settingsActive) {
			mainMenu.gameObject.SetActive (false);
			settingsMenu.gameObject.SetActive (true);
		} else {
			settingsMenu.gameObject.SetActive (false);
			mainMenu.gameObject.SetActive (true);
		}
	}

	public void quitGame() {
		Debug.Log ("Quit");
		Application.Quit ();
	}

	public void loadSave() {

	}

	public void setDifficultyEasy() {
		difficulty = "Easy";
	}

	public void setDifficultyMedium() {
		difficulty = "Medium";
	}

	public void setDifficultyHard() {
		difficulty = "Hard";
	}

	public void changeVolume() {
		Debug.Log ("Volume set to " + volumeSlider.value);
		AudioListener.volume = volumeSlider.value;
	}
}
