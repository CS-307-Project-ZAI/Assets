using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

	public GameManager gm;

	public string itemType = null;
	private float timer = 0.0f;
	public float timeOut = 30.0f;

	void Update() {
		if (this.gameObject.tag == "Material") {
			timer += Time.deltaTime;
			if (timer >= timeOut) {
				Destroy (this.gameObject);
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player") {
			if (this.gameObject.tag == "Material") {
				PersonController temp = other.transform.gameObject.GetComponent<PersonController> ();
				gm.player.addItem (itemType, 1);
				Destroy (this.gameObject);
			} else if (this.gameObject.tag == "WeaponPickup") {
				PersonController temp = other.transform.gameObject.GetComponent<PersonController> ();
				gm.player.addWeapon (itemType);
				Destroy (this.gameObject);
			}
		}
	}


}
