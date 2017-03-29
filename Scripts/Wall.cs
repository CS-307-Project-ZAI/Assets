using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour {

	public GameManager gm;
	public Wall wall;
	public string wallTier;
	public int wallHealth;
	public bool isBuilding = false;
	public int buildTime = 30;
	public bool kill = false;

	public int buildProgress = 0;

	float buildTickTime = 1.0f;
	float buildTickTimer = 0.0f;

	public List<PersonController> withinRange;

	// Use this for initialization
	void Start () {
		isBuilding = true;
	}

	void ApplyDamage(int dmg){
		isBuilding = false;
		this.wallHealth -= dmg;
		wallHealthCheck ();
	}

	public virtual void wallHealthCheck(){
		if (wallHealth <= 0) {
			//Destroy Wall GameObject
			kill = true;
		}
	}

	// Update is called once per frame
	public void GMUpdate () {

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
				Debug.Log (buildProgress);
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player" || other.gameObject.tag == "Ally")
		{
			Debug.Log ("Person came within range!");
			PersonController temp = other.transform.gameObject.GetComponent<PersonController>();
			if (!withinRange.Contains(temp))
				withinRange.Add(temp);
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Ally")
		{
			Debug.Log ("Person went out of range!");
			PersonController temp = collision.gameObject.GetComponent<PersonController>();
			withinRange.Remove(temp);
		}
	}
}
