using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : PersonController {

	public List<AllyController> allies;

	void Start() {
		base.Start ();
	}

	// GMUpdate is called by the GameManager once per frame
	public void GMUpdate () {
		getMovement ();
		getRotation ();
		foreach (Weapon w in weapons) {
			w.ControlledUpdate ();
		}
		if (weapons.Count > 0) {
			getActions ();
		}
		gm.ui.ammoLeft.text = weapons [currentWeapon].currentLoaded.ToString ();
		gm.ui.clipSize.text = weapons [currentWeapon].clipSize.ToString ();
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
		if (gm.playerMode == "Combat") { //Get actions for Combat mode
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

				if ((Input.GetKeyDown (KeyCode.R) || (Input.GetMouseButton (0) && weapons [currentWeapon].currentLoaded == 0))
				    && (weapons [currentWeapon].ammoPool > 0 || weapons [currentWeapon].ammoPool == -1)
				    && weapons [currentWeapon].clipSize != -1) {
					reloading = true;
				}
			} else {
				if (Input.GetMouseButton (0) && weapons [currentWeapon].currentLoaded > 0) {
					reloading = false;
					weapons [currentWeapon].SendMessage ("interruptReload");
					fireWeapon ();
				}

			}
			if (Input.GetMouseButton (1)) { //Player right-clicks in Combat mode
				
			}
		} else if (gm.playerMode == "Command") { //Get actions for Command mode
            
		} else if (gm.playerMode == "Build") { //Get actions for Build mode

		}
	}

	void addAlly(AllyController ally) {
		allies.Add (ally);
		ally.leader = this;
		ally.mode = AllyController.Mode.standstill;
	}

	void removeAlly(AllyController ally) {
		allies.Remove (ally);
	}

	public override void aliveCheck() {
		if (health <= 0) {
			print ("Game Over!");
		}
	}
}
