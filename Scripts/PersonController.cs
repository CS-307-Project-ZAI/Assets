using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonController : MonoBehaviour {

	public float moveSpeed = 0.2f;
	public List<Weapon> weapons;
	public int health = 50;
	public GameManager gm;
	public float rotationFix = 0.0f;
	public string personName = "Person";

	[HideInInspector]
	public bool reloading = false;
	public bool kill = false;
	public GameObject targetTag = null;

	public int currentWeapon = 0;
	protected float attackTimer = 0.0f;

	protected void Start() {
		//Give person starting weapon
		Weapon w = (Weapon)Instantiate (gm.startingWeapon);
		w.owner = this;
		string load = "AmmoTypes/" + w.ammoType;
		w.bullet = Resources.Load (load, typeof(Bullet)) as Bullet;
		weapons.Add (w);
		w.transform.parent = gameObject.transform;
	}

	protected void fireWeapon() {
		attackTimer += Time.deltaTime;
		if (attackTimer > weapons [currentWeapon].fireRate && weapons [currentWeapon].currentLoaded > 0) {
			weapons [currentWeapon].SendMessage ("fireWeapon");
			attackTimer = 0.0f;
		}
	}

	protected void fireWeaponAt(Vector3 point) {
		attackTimer += Time.deltaTime;
		if (attackTimer > weapons [currentWeapon].fireRate && weapons [currentWeapon].currentLoaded > 0) {
			//weapons [currentWeapon].SendMessage ("fireWeaponAt", point);
			weapons[currentWeapon].fireWeaponAt(point);
			attackTimer = 0.0f;
		}
	}

	void ApplyDamage(int dmg) {
		this.health -= dmg;
		if (this.health < 0) {
			health = 0;
		}
		aliveCheck ();
	}

	public virtual void aliveCheck() {
		if (health <= 0) {
			kill = true;
		}
	}
}
