using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIController : MonoBehaviour {

	GameManager gm;

	//Dictionary<string, ItemClass> inventory;

	Dictionary<string, int> inventory;

	//GameObject infoBoxLeft;
	GameObject combatBoxRight;
	GameObject commandBoxRight;
	GameObject buildBoxRight;
	RectTransform healthBar;

	[HideInInspector]
	RectTransform pauseMenu;
	Image tint;

	//Left info box
	Text modeText;
	Text clothCount;
	Text woodCount;
	Text stoneCount;
	Text metalCount;

	//Combat box right
	Text weapon;
	Text ammoPool;
	Text ammoLeft;
	RectTransform reloadBar;

	//Command box right
	Text selected;
	Dropdown selectedMode;
	Dropdown selectedAggression;
	Button waypointButton;
	Button waypointButtonDelete;

	//Build box right
	Dropdown buildingSelected;
	bool build = false;
	public PlacementDetector pd;

	//Health bar
	Text playerHealth;

	//Settings Menu
	bool settingsActive = false;
	RectTransform settingsMenu;
	Button easyButton;
	Button mediumButton;
	Button hardButton;
	Slider volumeSlider;

	//Save Overwrite Menu
	bool saveOverwriteActive = false;
	RectTransform saveOverwriteMenu;

	//GameOver Menu
	bool gameOverActive = false;
	RectTransform gameOverMenu;

	//Quit/Save Menu
	bool quitSaveActive = false;
	RectTransform quitSaveMenu;

    //quest log
    Text questLog1;
    Text questLog2;
    Text questLog3;
    Text questLog4;
    Text questLog5;

	//Weapon Select Box
	RectTransform[] slots = new RectTransform[6];
	RectTransform selectedWep;
	float[] selectPos = {2.5f, 37.5f, 72.5f, 107.5f, 142.5f, 177.5f};

	private bool uiPauseUpdated = true;

    private string activeGUI = "";
	private int prevWall = 0;

	public void GMStart() {
		gm = FindObjectOfType<GameManager> ();

		//Settings Menu
		tint = transform.Find ("Tint").GetComponent<Image> ();
		pauseMenu = transform.Find ("PauseMenu").GetComponent<RectTransform> ();
		settingsMenu = transform.Find ("SettingsMenu").GetComponent<RectTransform> ();
		easyButton = settingsMenu.transform.Find ("Difficulty_Easy").GetComponent<Button> ();
		mediumButton = settingsMenu.transform.Find ("Difficulty_Medium").GetComponent<Button> ();
		hardButton = settingsMenu.transform.Find ("Difficulty_Hard").GetComponent<Button> ();
		volumeSlider = settingsMenu.transform.Find ("Volume Slider").GetComponent<Slider> ();
		volumeSlider.value = ApplicationModel.volume;
		AudioListener.volume = ApplicationModel.volume;

		//Save Overwrite Menu
		saveOverwriteMenu = transform.Find("SaveOverwriteMenu").GetComponent<RectTransform> ();

		//Game Over Menu
		gameOverMenu = transform.Find("GameOver").GetComponent<RectTransform>();
		gameOverMenu.gameObject.SetActive (false);

		quitSaveMenu = transform.Find ("QuitMenu").GetComponent<RectTransform> ();

		//Info Box Left
		modeText = GameObject.Find ("Mode").GetComponent<Text>();
		clothCount = GameObject.Find ("Cloth_Count").GetComponent<Text> ();
		woodCount = GameObject.Find ("Wood_Count").GetComponent<Text> ();
		stoneCount = GameObject.Find ("Stone_Count").GetComponent<Text> ();
		metalCount = GameObject.Find ("Metal_Count").GetComponent<Text> ();

		//Combat box right
		combatBoxRight = transform.Find ("Combat Box Right").gameObject;
		weapon = combatBoxRight.transform.Find ("Weapon").GetComponent<Text>();
		ammoPool = combatBoxRight.transform.Find ("AmmoPool").GetComponent<Text> ();
		ammoLeft = combatBoxRight.transform.Find ("AmmoLeft").GetComponent<Text> ();
		reloadBar = combatBoxRight.transform.Find ("ReloadBar").GetComponent<RectTransform> ();

		//Command box right
		commandBoxRight = transform.Find ("Command Box Right").gameObject;
		selected = commandBoxRight.transform.Find ("Selected").GetComponent<Text> ();
		selectedMode = commandBoxRight.transform.Find ("Mode_Dropdown").GetComponent<Dropdown> ();
		waypointButton = commandBoxRight.transform.Find ("Waypoint_Button").GetComponent<Button> ();
		waypointButtonDelete = commandBoxRight.transform.Find ("Waypoint_Button_Delete").GetComponent<Button> ();
		selectedAggression = commandBoxRight.transform.Find ("Aggression_Dropdown").GetComponent<Dropdown> ();
		selectedMode.onValueChanged.AddListener(delegate {
			DropdownChangeMode(selectedMode);
		});
		selectedAggression.onValueChanged.AddListener(delegate {
			DropdownChangeAggression(selectedAggression);
		});

		//Build box right
		buildBoxRight = transform.Find ("Build Box Right").gameObject;
		buildingSelected = buildBoxRight.transform.Find ("Wall_Tier_Dropdown").GetComponent<Dropdown> ();
		buildingSelected.onValueChanged.AddListener (delegate {
			DropdownChangeWallTier(buildingSelected);
		});
			
		//Health bar
		playerHealth = GameObject.Find ("Health Amount").GetComponent<Text> ();
		healthBar = GameObject.Find ("Health Bar").GetComponent<RectTransform> ();

		//Quest Log
        questLog1 = GameObject.Find("log1").GetComponent<Text>();
        questLog2 = GameObject.Find("log2").GetComponent<Text>();
        questLog3 = GameObject.Find("log3").GetComponent<Text>();
        questLog4 = GameObject.Find("log4").GetComponent<Text>();
        questLog5 = GameObject.Find("log5").GetComponent<Text>();

		//Weapon Select Box
		slots[0] = GameObject.Find("Inventory Slot 1").GetComponent<RectTransform> ();
		slots[1] = GameObject.Find("Inventory Slot 2").GetComponent<RectTransform> ();
		slots[2] = GameObject.Find("Inventory Slot 3").GetComponent<RectTransform> ();
		slots[3] = GameObject.Find("Inventory Slot 4").GetComponent<RectTransform> ();
		slots[4] = GameObject.Find("Inventory Slot 5").GetComponent<RectTransform> ();
		slots[5] = GameObject.Find("Inventory Slot 6").GetComponent<RectTransform> ();
		selectedWep = GameObject.Find ("Selected Slot").GetComponent<RectTransform> ();

		for (int i = 0; i < 6; i++) {
			Image img = slots [i].FindChild ("WepImg").GetComponent<Image> ();
			img.color = new Color (1.0f, 1.0f, 1.0f, 0);
		}

		updateHealthBar ();
    }

	public void GMUpdate() {
		if (gm.gameOver) {
			if (!gameOverActive) {
				pauseMenu.gameObject.SetActive (false);
				settingsMenu.gameObject.SetActive (false);
				saveOverwriteMenu.gameObject.SetActive (false);
				quitSaveMenu.gameObject.SetActive (false);
				gameOverMenu.gameObject.SetActive (true);
			}
			return;
		}
		if (gm.paused) {
			if (!uiPauseUpdated) {
				tint.color = new Color (0, 0, 0, 0.8f);
				if (settingsActive) {
					pauseMenu.gameObject.SetActive (false);
					settingsMenu.gameObject.SetActive (true);
					saveOverwriteMenu.gameObject.SetActive (false);
					quitSaveMenu.gameObject.SetActive (false);
					switch (ApplicationModel.difficulty) {
					case "Easy":
						easyButton.interactable = false;
						mediumButton.interactable = true;
						hardButton.interactable = true;
						break;
					case "Medium":
						easyButton.interactable = true;
						mediumButton.interactable = false;
						hardButton.interactable = true;
						break;
					case "Hard":
						easyButton.interactable = true;
						mediumButton.interactable = true;
						hardButton.interactable = false;
						break;
					}
				} else if (saveOverwriteActive) {
					pauseMenu.gameObject.SetActive (false);
					settingsMenu.gameObject.SetActive (false);
					saveOverwriteMenu.gameObject.SetActive (true);
					quitSaveMenu.gameObject.SetActive (false);
				} else if (quitSaveActive) {
					pauseMenu.gameObject.SetActive (false);
					settingsMenu.gameObject.SetActive (false);
					saveOverwriteMenu.gameObject.SetActive (false);
					quitSaveMenu.gameObject.SetActive (true);
				} else {
					pauseMenu.gameObject.SetActive (true);
					settingsMenu.gameObject.SetActive (false);
					saveOverwriteMenu.gameObject.SetActive (false);
					quitSaveMenu.gameObject.SetActive (false);
				}
				uiPauseUpdated = true;
			}
			return;
		} else if (uiPauseUpdated) {
			tint.color = new Color (0.5f, 0.5f, 0.5f, 0.0f);
			pauseMenu.gameObject.SetActive (false);
			settingsMenu.gameObject.SetActive (false);
			saveOverwriteMenu.gameObject.SetActive (false);
			quitSaveMenu.gameObject.SetActive (false);
			uiPauseUpdated = false;
		}

		if (activeGUI != gm.playerMode) {
			activeGUI = gm.playerMode;
			switch (activeGUI) {
			case "Combat":
				combatBoxRight.SetActive (true);
				commandBoxRight.SetActive (false);
				buildBoxRight.SetActive (false);
				break;
			case "Command":
				combatBoxRight.SetActive (false);
				commandBoxRight.SetActive (true);
				buildBoxRight.SetActive (false);
				break;
			case "Build":
				combatBoxRight.SetActive (false);
				commandBoxRight.SetActive (false);
				buildBoxRight.SetActive (true);
				break;
			}
		}

		modeText.text = activeGUI;

		inventory = gm.player.playerInventory;
		//Counts items in playerItems and displays them
		clothCount.text = inventory ["cloth"].ToString ();
		woodCount.text = inventory ["wood"].ToString ();
		stoneCount.text = inventory ["stone"].ToString ();
		metalCount.text = inventory ["metal"].ToString ();


		if (!gm.build) {
			this.build = false;
			pd.gameObject.SetActive (false);
		} else if (activeGUI != "Build") {
			pd.gameObject.SetActive (false);
		} else {
			pd.gameObject.SetActive (true);
		}

		switch (activeGUI) {
		case "Combat":
			weapon.text = gm.player.weapons [gm.player.currentWeapon].weaponName;
			ammoLeft.text = gm.player.weapons [gm.player.currentWeapon].currentLoaded.ToString ();
			if (gm.player.weapons [gm.player.currentWeapon].ammoPool == -1) {
				ammoPool.text = "∞";
			} else {
				ammoPool.text = gm.player.weapons [gm.player.currentWeapon].ammoPool.ToString ();
			}
			if (gm.player.reloading) {
				float reloadWidth = (gm.player.weapons [gm.player.currentWeapon].reloadTimer / gm.player.weapons [gm.player.currentWeapon].reloadTime) * 180;
				reloadBar.sizeDelta = new Vector2 (reloadWidth, 20);
			} else {
				reloadBar.sizeDelta = new Vector2 (0, 20);
			}
			break;
		case "Command":
			if (gm.selectedAlly != null) {
				selected.text = gm.selectedAlly.personName;
				selectedMode.value = gm.selectedAlly.stats.modes.IndexOf (gm.selectedAlly.stats.mode);
				selectedAggression.value = gm.selectedAlly.stats.aggressions.IndexOf (gm.selectedAlly.stats.aggression);
				selectedMode.interactable = true;
				selectedAggression.interactable = true;
				waypointButton.interactable = true;
				waypointButtonDelete.interactable = true;
			} else {
				selected.text = "None";
				selectedMode.interactable = false;
				selectedAggression.interactable = false;
				waypointButton.interactable = false;
				waypointButtonDelete.interactable = false;
			}
			break;
		case "Build":
			if (gm.build) {
				if (!this.build) {
					this.build = true;
					pd.gameObject.SetActive (true);
				}
				Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				pd.transform.position = new Vector3 (mousePos.x, mousePos.y, 0);
				Vector3 playerRot = gm.player.transform.rotation.eulerAngles;
				pd.transform.eulerAngles = new Vector3 (0, 0, playerRot.z - gm.player.rotationFix + (gm.player.buildingRotation ? 90.0f : 0));
			}
			break;
		}

        if (gm.player.questLog == null) {
            Debug.Log("no quest log");
            return;
        }
        updateQuestLogText(questLog1, gm.player.questLog.questAt(0));
        updateQuestLogText(questLog2, gm.player.questLog.questAt(1));
        updateQuestLogText(questLog3, gm.player.questLog.questAt(2));
        updateQuestLogText(questLog4, gm.player.questLog.questAt(3));
        updateQuestLogText(questLog5, gm.player.questLog.questAt(4));
    }

	public void updateHealthBar() {
		//Update Health Bar
		playerHealth.text = gm.player.health.ToString();
		float healthWidth = ((float)gm.player.health / 50.0f) * 290;
		if (healthWidth < 0) {
			healthWidth = 0.0f;
		}
		healthBar.sizeDelta = new Vector2 (healthWidth, 30);
	}

    private void updateQuestLogText(Text t, Quest q)
    {
        if (q != null) {
            t.text = gm.player.questLog.questLogString(q);
        }
        else {
            t.text = "empty quest slot";
        }
    }

    private void DropdownChangeMode(Dropdown target) {
		if (gm.selectedAlly != null) {
			gm.selectedAlly.stats.mode = target.options[target.value].text;
		}
	}

	private void DropdownChangeAggression(Dropdown target) {
		if (gm.selectedAlly != null) {
			gm.selectedAlly.stats.aggression = target.options[target.value].text;
		}
	}

	private void DropdownChangeWallTier(Dropdown target) {
		gm.player.buildingSelected = target.value + 1;
		gm.player.checkMaterials = true;
		Debug.Log ("Building Selected Changed: " + gm.player.buildingSelected);
	}

	public void forceBuildingChange(int val) {
		buildingSelected.value = val;
		buildingSelected.RefreshShownValue ();
	}

	public void selectWeapon(int wep) {
		selectedWep.anchoredPosition = new Vector2 (2.5f + (wep * 35.0f), 0);
	}

	public void addWepImg(int slot, string wep) {
		Image img = slots [slot].FindChild ("WepImg").GetComponent<Image> ();
		Debug.Log (slot);
		string load = "WeaponImages/" + wep;
		img.sprite = Resources.Load(load, typeof(Sprite)) as Sprite;
		img.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
	}

	public void unpauseGame() {
		gm.paused = false;
		gm.cam.SetCustomCursor ();
	}

	public void openSettings() {
		settingsActive = true;
		uiPauseUpdated = false;
	}

	public void closeSettings() {
		settingsActive = false;
		uiPauseUpdated = false;
	}

	public void openQuitSave() {
		if (ApplicationModel.savefile != 0) {
			quitSaveActive = true;
			uiPauseUpdated = false;
		} else {
			gm.quitToMenu (0);
		}
	}

	public void closeQuitSave() {
		quitSaveActive = false;
		uiPauseUpdated = false;
	}

	public void saveQuit()  {
		gm.quitToMenu (1);
	}

	public void noSaveQuit() {
		gm.quitToMenu (0);
	}

	public void setDifficultyEasy() {
		ApplicationModel.difficulty = "Easy";
		uiPauseUpdated = false;
	}

	public void setDifficultyMedium() {
		ApplicationModel.difficulty = "Medium";
		uiPauseUpdated = false;
	}

	public void setDifficultyHard() {
		ApplicationModel.difficulty = "Hard";
		uiPauseUpdated = false;
	}

	public void attemptSave() {
		if (!gm.createSave ()) {
			saveOverwriteActive = true;
			uiPauseUpdated = false;
		}
	}

	public void overwriteSaveOne() {
		ApplicationModel.savefile = 1;
		saveOverwriteActive = false;
		uiPauseUpdated = false;
		gm.createSave ();
	}

	public void overwriteSaveTwo() {
		ApplicationModel.savefile = 2;
		saveOverwriteActive = false;
		uiPauseUpdated = false;
		gm.createSave ();
	}

	public void overwriteSaveThree() {
		ApplicationModel.savefile = 3;
		saveOverwriteActive = false;
		uiPauseUpdated = false;
		gm.createSave ();
	}

	public void saveBack() {
		saveOverwriteActive = false;
		uiPauseUpdated = false;
		ApplicationModel.savefile = 0;
	}

	public void changeVolume() {
		AudioListener.volume = volumeSlider.value;
		ApplicationModel.volume = volumeSlider.value;
	}
}
