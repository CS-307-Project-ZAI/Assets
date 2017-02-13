using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

	GameManager gm;

	//GameObject infoBoxLeft;
	//GameObject infoBoxRight;
	RectTransform healthBar;

	[HideInInspector]
	public Text modeText;
	public Text ammoPool;
	public Text ammoLeft;
	public Text playerHealth;

	public void GMStart() {
		gm = FindObjectOfType<GameManager> ();
		//infoBoxLeft = transform.Find ("Info Box Left").gameObject;
		//infoBoxRight = transform.Find ("Info Box Right").gameObject;
		modeText = GameObject.Find ("Mode").GetComponent<Text>();
		ammoPool = GameObject.Find ("AmmoPool").GetComponent<Text> ();
		ammoLeft = GameObject.Find ("AmmoLeft").GetComponent<Text> ();
		playerHealth = GameObject.Find ("Health Amount").GetComponent<Text> ();
		healthBar = GameObject.Find ("Health Bar").GetComponent<RectTransform> ();
	}

	public void GMUpdate() {
		ammoLeft.text = gm.player.weapons [gm.player.currentWeapon].currentLoaded.ToString ();
		if (gm.player.weapons [gm.player.currentWeapon].ammoPool == -1) {
			ammoPool.text = "∞";
		} else {
			ammoPool.text = gm.player.weapons [gm.player.currentWeapon].ammoPool.ToString ();
		}
		playerHealth.text = gm.player.health.ToString();
		float width = ((float)gm.player.health / 50.0f) * 260;
		if (width < 0) {
			width = 0.0f;
		}
		healthBar.sizeDelta = new Vector2 (width, 30);
	}
}
