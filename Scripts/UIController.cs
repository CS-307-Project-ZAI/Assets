using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

	GameManager gm;

	GameObject infoBox1;

	[HideInInspector]
	public Text modeText;

	public void GMStart() {
		gm = FindObjectOfType<GameManager> ();
		infoBox1 = transform.Find ("Info Box").gameObject;
		modeText = GameObject.Find ("Mode").GetComponent<Text>();
	}
}
