using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour {

	public GameManager gm;
	public string buildingName;
	public int buildingHealth;
	public int buildingMaxHealth;
	public bool isBuilding = false;
	public int buildTime = 30;
	public bool kill = false;
	public int buildProgress = 0;

	float buildTickTime = 1.0f;
	float buildTickTimer = 0.0f;

	public List<PersonController> withinRange;

	public static string[] materialNames = { "cloth", "wood", "stone", "metal" };
	public int[] materialsNeeded;

	public SpriteRenderer spr = null;
	private bool checkCondition = true;
	public Sprite[] conditionSprites;

	// Use this for initialization
	void Awake () {
		spr = this.gameObject.GetComponent<SpriteRenderer> ();
		Debug.Log (spr);
		isBuilding = true;
	}

	public void GMUpdate() {
		//Update Building
		if (isBuilding) {
			buildTickTimer += (buildTickTimer >= buildTickTime ? 0 : Time.deltaTime);
			if (buildTickTimer >= buildTickTime && withinRange.Count > 0) {
				buildTickTimer = 0.0f;
				foreach (PersonController p in withinRange) {
					buildProgress += p.getBuildRate ();
				}
				if (buildProgress >= buildTime) {
					buildProgress = buildTime;
					isBuilding = false;
				}
				//Debug.Log (buildProgress);
			}
		} else if (checkCondition && this.gameObject.tag == "Wall") {
			if (buildingHealth >= 0.666f * buildingMaxHealth) {
				spr.sprite = conditionSprites [0];
			} else if (buildingHealth >= 0.333f * buildingMaxHealth) {
				spr.sprite = conditionSprites [1];
			} else {
				spr.sprite = conditionSprites [2];
			}
			checkCondition = false;
		}
	}
	
	void ApplyDamage(int dmg) {
		isBuilding = false;
		this.buildingHealth -= dmg;
		buildingHealthCheck ();
		checkCondition = true;
	}

	public virtual void buildingHealthCheck() {
		if (buildingHealth <= 0) {
			//Destroy Building GameObject
			kill = true;
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player" || other.gameObject.tag == "Ally") {
			//Debug.Log ("Person came within range!");
			PersonController temp = other.transform.gameObject.GetComponent<PersonController> ();
			if (!withinRange.Contains (temp))
				withinRange.Add (temp);
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Ally") {
			//Debug.Log ("Person went out of range!");
			PersonController temp = collision.gameObject.GetComponent<PersonController>();
			withinRange.Remove(temp);
		}
	}

	private void OnCollisionEnter2D(Collision2D col) {
		if (col.gameObject.tag == "Detector") {
			//Debug.Log ("Enter!");
			gm.ui.pd.addCollision (this.gameObject);
		}
	}

	private void OnCollisionExit2D(Collision2D col) {
		if (col.gameObject.tag == "Detector") {
			//Debug.Log ("Exit!");
			gm.ui.pd.removeCollision (this.gameObject);
		}
	}
}
