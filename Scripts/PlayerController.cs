using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float moveSpeed = 0.2f;
	public List<Weapon> weapons;
	public int health = 50;
	public GameManager gm;

	[HideInInspector]
	public float rotationFix = 45.0f;
	int currentWeapon = 0;
	float attackTimer = 0.0f;
	public bool reloading = false;

	// Use this for initialization
	void Start () {
		//Give player starting weapon
		Weapon w = (Weapon)Instantiate (gm.startingWeapon);
		w.owner = this;
		string load = "AmmoTypes/" + w.ammoType;
		print (load);
		w.bullet = Resources.Load (load, typeof(Bullet)) as Bullet; 
		print (w.bullet);
		weapons.Add (w);
		w.transform.parent = gameObject.transform;
	}
	
	// Update is called once per frame
	void Update () {
		getMovement ();
		getRotation ();
		if (weapons.Count > 0) {
			getActions ();
		}
	}

	void getMovement() {
		float moveX = Input.GetAxisRaw ("Horizontal");
		float moveY = Input.GetAxisRaw ("Vertical");
		Vector3 movement = new Vector3 (moveX * moveSpeed, moveY * moveSpeed, 0);
		transform.position += Vector3.ClampMagnitude(movement, moveSpeed) * Time.deltaTime;
	}

	void getRotation() {
		Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		transform.rotation = Quaternion.LookRotation (Vector3.forward, mousePos - transform.position);
		transform.Rotate (new Vector3 (0, 0, rotationFix));
	}

	void getActions() {
		if (!reloading) {
			//Changing Weapons
			if (Input.GetKeyDown (KeyCode.Alpha1) && weapons.Count > 0) {
				currentWeapon = 0;
				return;
			} else if (Input.GetKeyDown (KeyCode.Alpha2) && weapons.Count > 1) {
				currentWeapon = 1;
				return;
			} else if (Input.GetKeyDown (KeyCode.Alpha3) && weapons.Count > 2) {
				currentWeapon = 2;
				return;
			}
			
			if (Input.GetMouseButton (0)) {
				fireWeapon ();
			} else {
				if (attackTimer < weapons [currentWeapon].fireRate) {
					attackTimer += Time.deltaTime;
				} else {
					attackTimer = weapons [currentWeapon].fireRate;
				}
			}

			if ((Input.GetKeyDown (KeyCode.R) || (Input.GetMouseButton(0) && weapons[currentWeapon].currentLoaded == 0))
				&& (weapons[currentWeapon].ammoPool > 0 || weapons[currentWeapon].ammoPool == -1)
				&& weapons[currentWeapon].clipSize != -1) {
				reloading = true;
			}
		} else {
			if (Input.GetMouseButton (0) && weapons[currentWeapon].currentLoaded > 0) {
				reloading = false;
				weapons [currentWeapon].SendMessage ("interruptReload");
				fireWeapon ();
			}
		}
	}

	void fireWeapon() {
		attackTimer += Time.deltaTime;
		if (attackTimer > weapons [currentWeapon].fireRate && weapons [currentWeapon].currentLoaded > 0) {
			weapons [currentWeapon].SendMessage ("fireWeapon");
			attackTimer = 0.0f;
		}
	}

	void aliveCheck() {
		if (health <= 0) {
			print ("Game Over!");
		}
	}

	void ApplyDamage(int dmg) {
		this.health -= dmg;
		aliveCheck ();
	}
}
