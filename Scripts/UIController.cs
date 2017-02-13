using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

	GameManager gm;

	GameObject infoBoxLeft;
	GameObject infoBoxRight;

	[HideInInspector]
	public Text modeText;
	public Text clipSize;
	public Text ammoLeft;

	public void GMStart() {
		gm = FindObjectOfType<GameManager> ();
		infoBoxLeft = transform.Find ("Info Box Left").gameObject;
		infoBoxRight = transform.Find ("Info Box Right").gameObject;
		modeText = GameObject.Find ("Mode").GetComponent<Text>();
		clipSize = GameObject.Find ("ClipSize").GetComponent<Text> ();
		ammoLeft = GameObject.Find ("AmmoLeft").GetComponent<Text> ();
	}
}
