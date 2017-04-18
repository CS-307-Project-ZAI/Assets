using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuManager : GameManager {

	public AllyController dummyAlly;
	bool settingsActive = false;
	bool loadActive = false;

	public float spawnTimer = 0.0f;
	public float spawnTime = 3.0f;

	public RectTransform mainMenu;
	public RectTransform settingsMenu;
	Button easyButton;
	Button mediumButton;
	Button hardButton;
	Slider volumeSlider;

	public RectTransform loadMenu;
	Button loadOneButton;
	Button loadTwoButton;
	Button loadThreeButton;

	// Use this for initialization
	void Start () {
		ApplicationModel.savePath = Application.streamingAssetsPath + "/saves/";
		mainMenu.gameObject.SetActive (true);
		settingsMenu.gameObject.SetActive (false);
		easyButton = settingsMenu.transform.Find ("Difficulty_Easy").GetComponent<Button> ();
		mediumButton = settingsMenu.transform.Find ("Difficulty_Medium").GetComponent<Button> ();
		hardButton = settingsMenu.transform.Find ("Difficulty_Hard").GetComponent<Button> ();
		volumeSlider = settingsMenu.Find ("Volume Slider").GetComponent<Slider>();
		loadMenu.gameObject.SetActive (false);
		loadOneButton = loadMenu.transform.Find ("Load_One_Button").GetComponent<Button> ();
		loadTwoButton = loadMenu.transform.Find ("Load_Two_Button").GetComponent<Button> ();
		loadThreeButton = loadMenu.transform.Find ("Load_Three_Button").GetComponent<Button> ();
		setDifficultyEasy ();

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
		ApplicationModel.savefile = 0;
		SceneManager.LoadSceneAsync ("Scenes/GameScene");
	}

	public void toggleSettings() {
		settingsActive = !settingsActive;
		if (settingsActive) {
			mainMenu.gameObject.SetActive (false);
			settingsMenu.gameObject.SetActive (true);
			loadMenu.gameObject.SetActive (false);
		} else {
			settingsMenu.gameObject.SetActive (false);
			mainMenu.gameObject.SetActive (true);
			loadMenu.gameObject.SetActive (false);
		}
	}

	public void toggleLoad() {
		loadActive = !loadActive;
		if (loadActive) {
			mainMenu.gameObject.SetActive (false);
			settingsMenu.gameObject.SetActive (false);
			loadMenu.gameObject.SetActive (true);
			if (System.IO.File.Exists (ApplicationModel.savePath + "savefile1.txt")) {
				loadOneButton.interactable = true;
			} else {
				loadOneButton.interactable = false;
			}
			if (System.IO.File.Exists (ApplicationModel.savePath + "savefile2.txt")) {
				loadTwoButton.interactable = true;
			} else {
				loadTwoButton.interactable = false;
			}
			if (System.IO.File.Exists (ApplicationModel.savePath + "savefile3.txt")) {
				loadThreeButton.interactable = true;
			} else {
				loadThreeButton.interactable = false;
			}
		} else {
			settingsMenu.gameObject.SetActive (false);
			mainMenu.gameObject.SetActive (true);
			loadMenu.gameObject.SetActive (false);
		}
	}

	public void quitGame() {
		Debug.Log ("Quit");
		Application.Quit ();
	}

	public void loadSave(string file) {
		Debug.Log ("Load: " + file);
		SceneManager.LoadSceneAsync ("Scenes/GameScene");
	}

	public void setDifficultyEasy() {
		ApplicationModel.difficulty = "Easy";
		easyButton.interactable = false;
		mediumButton.interactable = true;
		hardButton.interactable = true;
	}

	public void setDifficultyMedium() {
		ApplicationModel.difficulty = "Medium";
		easyButton.interactable = true;
		mediumButton.interactable = false;
		hardButton.interactable = true;
	}

	public void setDifficultyHard() {
		ApplicationModel.difficulty = "Hard";
		easyButton.interactable = true;
		mediumButton.interactable = true;
		hardButton.interactable = false;
	}

	public void changeVolume() {
		Debug.Log ("Volume set to " + volumeSlider.value);
		ApplicationModel.volume = volumeSlider.value;
		AudioListener.volume = volumeSlider.value;
	}

	public void loadSaveOne() {
		ApplicationModel.savefile = 1;
		loadSave (ApplicationModel.savePath + "savefile1.txt");
	}

	public void loadSaveTwo() {
		ApplicationModel.savefile = 2;
		loadSave (ApplicationModel.savePath + "savefile2.txt");
	}

	public void loadSaveThree() {
		ApplicationModel.savefile = 3;
		loadSave (ApplicationModel.savePath + "savefile3.txt");
	}
}
