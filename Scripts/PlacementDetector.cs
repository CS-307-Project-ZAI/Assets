using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementDetector : MonoBehaviour {

	SpriteRenderer spr;
	public Sprite green;
	public Sprite red;

	public List<GameObject> collisions = new List<GameObject>();

	void Start() {
		spr = gameObject.GetComponent<SpriteRenderer> ();
	}

	void OnCollisionEnter2D(Collision2D col) {
		Debug.Log ("Collided!");
		collisions.Add (col.gameObject);
		spr.sprite = red;
	}

	void OnCollisionExit2D(Collision2D col) {
		Debug.Log ("Remove!");
		collisions.Remove (col.gameObject);
		if (collisions.Count <= 0) {
			spr.sprite = green;
		}
	}

	public bool checkCollision() {
		if (collisions.Count <= 0) {
			return false;
		}
		return true;
	}
}
