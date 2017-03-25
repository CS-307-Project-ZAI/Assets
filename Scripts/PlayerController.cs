using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : PersonController {

	public List<AllyController> allies;
	public bool wallRotation = false;
	public int buildRate = 1;

	new void Start() {
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
			if (Input.GetMouseButtonDown (1)) { //Player right-clicks in Combat mode
				GameObject obj = gm.getClickedObject ();
				if (obj != null) {
					if (obj.tag == "Enemy") {
						//Target enemy
						Debug.Log("Enemy targeted");
						gm.targetEnemy (obj.GetComponent<EnemyController> ());
					}
				}
			}
		} else if (gm.playerMode == "Command") { //Get actions for Command mode
			if (Input.GetMouseButtonDown (0)) {
				if (gm.setWaypoints && gm.selectedAlly != null) {
					Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
					gm.selectedAlly.addWaypoint (mousePos);
				} else {
					GameObject obj = gm.getClickedObject ();
					if (obj != null) {
						if (obj.tag == "Ally") {
							gm.selectedAlly = obj.GetComponent<AllyController> ();
						} else if (!EventSystem.current.IsPointerOverGameObject ()) {
							gm.deselectAlly ();
						}
					} else if (!EventSystem.current.IsPointerOverGameObject ()) {
						gm.deselectAlly ();
					}
				}
			} else if (Input.GetMouseButtonDown (1)) {
				if (gm.selectedAlly != null) {
					if (gm.setWaypoints) {
						Debug.Log ("Removing waypoint");
						GameObject obj = gm.getClickedObject ();
						if (obj != null) {
							if (obj.tag == "Waypoint") {
								gm.selectedAlly.movePoints.Remove (obj.transform.position);
								Destroy (obj);
							}
						}
					} else {
						gm.selectedAlly.commandMove (Camera.main.ScreenToWorldPoint (Input.mousePosition));
					}
				}
			} else if (Input.GetKeyDown (KeyCode.Q) && gm.selectedAlly != null) {
				gm.selectedAlly.removeLastWaypoint ();
			} else if (Input.GetKeyDown (KeyCode.E) && gm.selectedAlly != null) {
				gm.toggleWaypoints ();
			}
		} else if (gm.playerMode == "Build") { //Get actions for Build mode
			//Will Change Wall according to UI Wall Tier selected in the dropdown
			string loadWall = "Walls/" + wall.wallTier;
			wall = Resources.Load(loadWall, typeof(Wall)) as Wall;
			if (Input.GetKeyDown(KeyCode.R)) {
				wallRotation = !wallRotation;
			}
			if (Input.GetKeyDown (KeyCode.E)) {
				gm.toggleBuild (false);
			}
			if (Input.GetKeyDown (KeyCode.Q)) {
				gm.toggleBuildDestroy ();
			}
			if (gm.build && Input.GetMouseButtonDown (0)) {
				Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				Wall newWall = (Wall)Instantiate (
					wall, new Vector3 (mousePos.x, mousePos.y, 0), 
					Quaternion.LookRotation (Vector3.forward, mousePos - transform.position));
				newWall.gm = gm;
				gm.walls.Add (newWall);
				if (wallRotation) {
					newWall.transform.Rotate (new Vector3 (0, 0, 90.0f));
				}
				gm.toggleBuild (true);
			}
			if (gm.buildDestroy && Input.GetMouseButtonDown (0)) {
				GameObject obj = gm.getClickedObject ();
				if (obj != null) {
					if (obj.tag == "Wall") {
						gm.walls.Remove((Wall) obj.GetComponent<Wall>());
						Destroy (obj);
						gm.recreateGrid ();
					}
				}
			}
		}
	}

	void addAlly(AllyController ally) {
		allies.Add (ally);
		ally.leader = this;
		ally.mode = "Standstill";
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
