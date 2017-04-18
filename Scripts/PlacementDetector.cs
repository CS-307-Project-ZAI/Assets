using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementDetector : MonoBehaviour {

	public GameManager gm;
	SpriteRenderer spr;
	public Sprite[] buildings;
	public int currentSelected = 1;
	public LayerMask checkLayer;

	public List<GameObject> collisions = new List<GameObject>();
	List<GameObject> removal = new List<GameObject>();
	public bool check = true;

	private BoxCollider2D bc2d;

	void Start() {
		spr = gameObject.GetComponent<SpriteRenderer> ();
		bc2d = gameObject.GetComponent<BoxCollider2D> ();
	}

	void FixedUpdate() {
		foreach (GameObject obj in removal) {
			collisions.Remove (obj);
		}
		removal.Clear ();
		if (currentSelected != gm.player.buildingSelected && gm.player.buildingSelected < (buildings.Length + 1)) {
			if (currentSelected == 4) {
				bc2d.size = new Vector2 (1.4f, 0.7f);
			} else if (gm.player.buildingSelected == 4) {
				bc2d.size = new Vector2 (0.7f, 0.7f);
			}
			currentSelected = gm.player.buildingSelected;
			spr.sprite = buildings [currentSelected - 1];
		}
		checkPlacement();
	}

	public void addCollision(GameObject obj) {
		if (!collisions.Contains(obj)) {
			collisions.Add(obj);
		}
		checkPlacement();
	}

	public void removeCollision(GameObject obj) {
		removal.Add (obj);
	}
		
	public bool checkPlacement() {
		if (collisions.Count <= 0 && gm.player.enoughMaterials) {
			check = true; //True, we can place a wall here
			spr.color = new Color(0.2f, 1.0f, 0.2f, 0.5f);
			return true;
		}
		check = false; //False, cannot place a wall here
		spr.color = new Color(1.0f, 0.2f, 0.2f, 0.5f);
		return false;
	}
}
