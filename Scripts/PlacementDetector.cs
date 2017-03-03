using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementDetector : MonoBehaviour {

	bool check = false;
	SpriteRenderer spr;
	public Sprite green;
	public Sprite red;
	public LayerMask checkLayer;

	public List<GameObject> collisions = new List<GameObject>();

	void Start() {
		spr = gameObject.GetComponent<SpriteRenderer> ();
	}

	void OnTriggerEnter2D(Collider2D col) {
		if (col.gameObject.tag == "Wall" || col.gameObject.tag == "Block") {
			Debug.Log ("Enter");
			Debug.Log ("Count: " + collisions.Count);
			if (!collisions.Contains (col.gameObject)) {
				collisions.Add (col.gameObject);
			}
		}
	}

	void OnTriggerExit2D(Collider2D col) {
		if (col.gameObject.tag == "Wall" || col.gameObject.tag == "Block") {
			Debug.Log ("Exit");
			Debug.Log ("Count: " + collisions.Count);
			collisions.Remove (col.gameObject);
		}
	}

	public bool checkCollision() {
		if (collisions.Count <= 0) {
			return false;
		}
		return true;
	}
}
