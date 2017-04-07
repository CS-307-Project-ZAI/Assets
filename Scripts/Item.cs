using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

	public GameManager gm;

	public string itemType;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame


	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player") {
			Debug.Log ("Person came within range of item!");
			PersonController temp = other.transform.gameObject.GetComponent<PersonController> ();
			gm.player.playerInventory [itemType]++;
			Destroy (this.gameObject);
		}
	}


}
