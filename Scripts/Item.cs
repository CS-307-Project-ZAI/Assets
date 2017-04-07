using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

	public GameManager gm;

	public string itemType;

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player") {
			PersonController temp = other.transform.gameObject.GetComponent<PersonController> ();
			gm.player.playerInventory [itemType]++;
			Destroy (this.gameObject);
		}
	}


}
