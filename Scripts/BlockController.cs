using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour {
	GameManager gm;

	void Start() {
		gm = FindObjectOfType<GameManager> ();
	}

	private void OnCollisionEnter2D(Collision2D col) {
		if (col.gameObject.tag == "Detector") {
			Debug.Log ("Enter!");
			gm.ui.pd.addCollision (this.gameObject);
		}
	}

	private void OnCollisionExit2D(Collision2D col) {
		if (col.gameObject.tag == "Detector") {
			Debug.Log ("Exit!");
			gm.ui.pd.removeCollision (this.gameObject);
		}
	}
}
