using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementDetector : MonoBehaviour {

	SpriteRenderer spr;
	public Sprite green;
	public Sprite red;
	public LayerMask checkLayer;

	public List<GameObject> collisions = new List<GameObject>();
	List<GameObject> removal = new List<GameObject>();
	public bool check = true;

	void Start() {
		spr = gameObject.GetComponent<SpriteRenderer> ();
	}

	void FixedUpdate() {
		foreach (GameObject obj in removal) {
			collisions.Remove (obj);
		}
		removal.Clear ();
		checkCollision ();
	}

	public void addCollision(GameObject obj) {
		if (!collisions.Contains(obj)) {
			collisions.Add(obj);
		}
		checkCollision ();
	}

	public void removeCollision(GameObject obj) {
		removal.Add (obj);
	}
		
	public void checkCollision() {
		if (collisions.Count <= 0) {
			check = true; //True, we can place a wall here
			spr.sprite = green;
			return;
		}
		check = false; //False, cannot place a wall here
		spr.sprite = red;
	}
}
