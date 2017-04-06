using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollider : MonoBehaviour {

	public EnemyController owner;

	public void setRadius(float radius) {
		transform.GetComponent<CircleCollider2D> ().radius = radius;
	}

	void OnTriggerStay2D(Collider2D col) {
		if (col.gameObject.tag == "Player") {
			if (owner.attackTimer >= owner.attackRate) {
				col.gameObject.GetComponent<PersonController>().applyDamage (owner.damage, owner);
				owner.attackTimer = 0.0f;
			}
		} else if (col.gameObject.tag == "Ally") {
			if (owner.attackTimer >= owner.attackRate) {
				col.gameObject.GetComponent<PersonController>().applyDamage (owner.damage, owner);
				owner.attackTimer = 0.0f;
			}
		} else if (col.gameObject.tag == "Wall") {
			if (owner.attackTimer >= owner.attackRate) {
				col.gameObject.SendMessage ("ApplyDamage", owner.damage);
				owner.attackTimer = 0.0f;
			}
		}
	}

}
