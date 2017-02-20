using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

	GameManager gm;

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

	//Combat box right
	Text weapon;
	Text ammoPool;
	Text ammoLeft;
	RectTransform reloadBar;

	//Command box right
	Text selected;
	Dropdown selectedMode;
	Dropdown selectedAggression;

	//Health bar
	Text playerHealth;

	//Settings Menu
	bool settingsActive = false;
	RectTransform settingsMenu;
	Button easyButton;
	Button mediumButton;
	Button hardButton;
	Slider volumeSlider;

	private string activeGUI = "";

	public void GMStart() {
		gm = FindObjectOfType<GameManager> ();

		tint = transform.Find ("Tint").GetComponent<Image> ();
		pauseMenu = transform.Find ("PauseMenu").GetComponent<RectTransform> ();
		settingsMenu = transform.Find ("SettingsMenu").GetComponent<RectTransform> ();
		easyButton = settingsMenu.transform.Find ("Difficulty_Easy").GetComponent<Button> ();
		mediumButton = settingsMenu.transform.Find ("Difficulty_Medium").GetComponent<Button> ();
		hardButton = settingsMenu.transform.Find ("Difficulty_Hard").GetComponent<Button> ();
		volumeSlider = settingsMenu.transform.Find ("Volume Slider").GetComponent<Slider> ();
		AudioListener.volume = volumeSlider.value;

		//infoBoxLeft = transform.Find ("Info Box Left").gameObject;
		modeText = GameObject.Find ("Mode").GetComponent<Text>();

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
		selectedAggression = commandBoxRight.transform.Find ("Aggression_Dropdown").GetComponent<Dropdown> ();
		selectedMode.onValueChanged.AddListener(delegate {
			DropdownChangeMode(selectedMode);
		});
		selectedAggression.onValueChanged.AddListener(delegate {
			DropdownChangeAggression(selectedAggression);
		});

		//Build box right
		buildBoxRight = transform.Find ("Build Box Right").gameObject;

		//Health bar
		playerHealth = GameObject.Find ("Health Amount").GetComponent<Text> ();
		healthBar = GameObject.Find ("Health Bar").GetComponent<RectTransform> ();
	}

	public void GMUpdate() {
		if (gm.paused) {
			tint.color = new Color (0, 0, 0, 0.8f);
			if (settingsActive) {
				pauseMenu.gameObject.SetActive (false);
				settingsMenu.gameObject.SetActive (true);
				switch (gm.difficulty) {
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
			} else {
				pauseMenu.gameObject.SetActive (true);
				settingsMenu.gameObject.SetActive (false);
			}
			return;
		}
		tint.color = new Color (0.5f, 0.5f, 0.5f, 0.0f);
		pauseMenu.gameObject.SetActive (false);
		settingsMenu.gameObject.SetActive (false);

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
				selectedMode.value = gm.selectedAlly.modes.IndexOf (gm.selectedAlly.mode);
				selectedAggression.value = gm.selectedAlly.aggressions.IndexOf (gm.selectedAlly.aggression);
				selectedMode.interactable = true;
				selectedAggression.interactable = true;
			} else {
				selected.text = "None";
				selectedMode.interactable = false;
				selectedAggression.interactable = false;
			}
			break;
		case "Build":
			break;
		}

		//Update Health Bar
		playerHealth.text = gm.player.health.ToString();
		float healthWidth = ((float)gm.player.health / 50.0f) * 290;
		if (healthWidth < 0) {
			healthWidth = 0.0f;
		}
		healthBar.sizeDelta = new Vector2 (healthWidth, 30);
	}

	private void DropdownChangeMode(Dropdown target) {
		if (gm.selectedAlly != null) {
			gm.selectedAlly.mode = target.options[target.value].text;
		}
	}

	private void DropdownChangeAggression(Dropdown target) {
		if (gm.selectedAlly != null) {
			gm.selectedAlly.aggression = target.options[target.value].text;
		}
	}

	public void unpauseGame() {
		gm.paused = false;
		gm.cam.SetCustomCursor ();
	}

	public void openSettings() {
		settingsActive = true;
	}

	public void closeSettings() {
		settingsActive = false;
	}

	public void quitGame() {
		Debug.Log ("Quit");
		Application.Quit ();
	}

	public void setDifficultyEasy() {
		gm.difficulty = "Easy";
	}

	public void setDifficultyMedium() {
		gm.difficulty = "Medium";
	}

	public void setDifficultyHard() {
		gm.difficulty = "Hard";
	}

	public void changeVolume() {
		AudioListener.volume = volumeSlider.value;
	}
}
