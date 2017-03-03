using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour {

	public GameManager gm;
	public Wall wall;
	public string wallTier;
	public int wallHealth;


	// Use this for initialization
	void Start () {
		
	}

	void ApplyDamage(int dmg){
		this.wallHealth -= dmg;
		if (this.wallHealth < 0) {
			this.wallHealth = 0;
		}
		wallHealthCheck ();
	}

	public virtual void wallHealthCheck(){
		if (wallHealth <= 0) {
			//Destroy Wall GameObject
			Destroy (this.gameObject);
			gm.recreateGrid ();
		}
	}


	// Update is called once per frame
	void Update () {
		
	}
}
