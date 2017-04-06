using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : PersonController {

	public List<AllyController> allies;
	public bool wallRotation = false;
	public int buildRate = 1;
    public QuestLog questLog;


    new void Start() {
		base.Start ();

		questLog = GetComponent<QuestLog>();
		questLog.questLogOwner = this;
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
				GameObject obj = gm.getClickedObject (2);
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
					GameObject obj = gm.getClickedObject (0);
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
						GameObject obj = gm.getClickedObject (0);
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

			//Wall recipes (TEMP):
			//Tier1Wall: 5 Cloth
			//Tier2Wall: 5 Wood
			//Tier3Wall: 5 Metal

			//Temp Commands
			//Add materials to inventory

			if (Input.GetKeyDown (KeyCode.I)) {
				playerItems ["cloth"]++;
			}
			if (Input.GetKeyDown (KeyCode.O)) {
				playerItems ["wood"]++;
			}
			if (Input.GetKeyDown (KeyCode.P)) {
				playerItems ["metal"]++;
			}

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
			//Debug.Log ("wall.wallTier: " + wall.wallTier);
			bool enoughMaterials = true;

			switch (wall.wallTier) {
				case "Tier1Wall":
					if (playerItems ["cloth"] < 5) {
						//Debug.Log ("Not enough cloth!");
						enoughMaterials = false;
					}
					break;
				case "Tier2Wall":
					if (playerItems ["wood"] < 5) {
						//Debug.Log ("Not enough wood!");
						enoughMaterials = false;
					}
					break;
				case "Tier3Wall":
					if (playerItems ["metal"] < 5) {
						//Debug.Log ("Not enough metal");
						enoughMaterials = false;
					}
					break;
			default:
				enoughMaterials = true;
				break;
			}

			if (gm.build && Input.GetMouseButtonDown (0) && enoughMaterials && gm.ui.pd.check) {
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

				//after wall have been built
				//need to remove items from inventory

				switch (wall.wallTier) {
				case "Tier1Wall":
					playerItems ["cloth"] -= 5;
					break;
				case "Tier2Wall":
					playerItems ["wood"] -= 5;
					break;
				case "Tier3Wall":
					playerItems ["metal"] -= 5;
					break;
				default:
					break;
				}
			}

			if (gm.buildDestroy && Input.GetMouseButtonDown (0)) {
				GameObject obj = gm.getClickedObject (1);
				if (obj != null) {
					if (obj.tag == "Wall") {
						gm.walls.Remove((Wall) obj.GetComponent<Wall>());
						Destroy (obj);
						gm.recreateGrid ();
					}
				}
			}
		}

        if (Input.GetKeyDown(KeyCode.F))
        {
            AllyController a = getMeleeAlly();
            if (a != null)
            {
                handleQuestInput(a);
            }
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            printQuestLog();
        }
    }

    public void printQuestLog()
    {
        foreach (Quest q in questLog.quests)
        {
            Debug.Log(q.getLogString());
        }
    }

    public AllyController getMeleeAlly()
    {
        foreach (AllyController a in gm.people)
        {
            if (Vector3.Magnitude(a.transform.position - transform.position) <= 3f)
            {
                if (a.questToGive != null && a.leader == null)
                {
                    return a;
                }
            }
        }
        return null;
    }

    private void handleQuestInput(AllyController a)
    {
        if (questLog.quests.Contains(a.questToGive))
        {
            questLog.turninQuest(a, a.questToGive.getQuestID());
        }
        else
        {
            a.assignQuest(questLog);
        }
    }

    public void addAlly(AllyController ally) {
		allies.Add (ally);
		ally.leader = this;
		ally.stats.mode = "Standstill";
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


