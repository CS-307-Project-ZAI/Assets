using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyController : PersonController {

	public PlayerController leader;
	public string mode = "Standstill";
	private string prevMode = "Standstill";
	public string aggression = "Defensive";

	public List<Vector3> movePoints;
	public Vector3 lookPoint;
	public Vector3 actionPoint;
	public bool cyclic = true;
	public float waitTime;
	[Range(0,2)]
	public float easeAmount;
	float nextMoveTime;
	float percentBetweenPoints;
	int fromPoint;
	int toPoint;

	bool onPath = false;
	public float actionDelay = 0.5f;
	float actionTimer = 0.0f;
	bool performingAction = false;
	public Vector3 previousPosition;
	bool positionFix = false;
	public float flightDistance = 1.0f;

	[HideInInspector]
	public List<string> modes = new List<string> {"Standstill", "Points", "Wander"};
	public List<string> aggressions = new List<string> {"Passive", "Defensive", "Offensive"};

	// Use this for initialization
	new void Start () {
		base.Start ();
		previousPosition = movePoints [0];
		if (mode == "Points") {
			positionFix = true;
			onPath = false;
		}
	}
	
	// Update is called once per frame
	public void GMUpdate () {
		if (prevMode != mode) {
			prevMode = mode;
			if (mode == "Points") {
				positionFix = true;
				onPath = false;
			}
		}
		getActions ();
		if (!performingAction) {
			getMovement ();
			getRotation ();
		} else {
			transform.rotation = Quaternion.LookRotation (Vector3.forward, actionPoint - transform.position);
			transform.Rotate (new Vector3 (0, 0, rotationFix));
		}
		foreach (Weapon w in weapons) {
			w.ControlledUpdate ();
		}
	}

	void getMovement() {
		switch (mode) {
		case "Standstill":
			break;
		case "Points":
			if (!performingAction) {
				Vector3 direction = Vector3.zero;
				if (positionFix) {
					float dirX = previousPosition.x - transform.position.x;
					float dirY = previousPosition.y - transform.position.y;
					if (Mathf.Abs (dirX) < .001 && Mathf.Abs (dirY) < .001) {
						positionFix = false;
					}
					Vector3 dir = new Vector3(dirX, dirY, 0);
					direction = Vector3.ClampMagnitude(dir * 1000, moveSpeed) * Time.deltaTime;
				} else {
					direction = CalculatePointMovement ();
					if (onPath) {
						previousPosition = transform.position;
					}
				}
				transform.position += direction;
				if (direction != Vector3.zero) {
					lookPoint = direction;
				}
			}
			break;
		case "Wander":
			break;
		}
	}

	Vector3 CalculatePointMovement() {
		if (Time.time < nextMoveTime) {
			return Vector3.zero;
		}
		onPath = true;
		fromPoint %= movePoints.Count;
		toPoint = (fromPoint + 1) % movePoints.Count;
		float distanceBetweenPoints = Vector3.Distance (movePoints[fromPoint], movePoints[toPoint]);
		percentBetweenPoints += Time.deltaTime * moveSpeed/distanceBetweenPoints;
		percentBetweenPoints = Mathf.Clamp01 (percentBetweenPoints);
		float easedPercentBetweenWaypoints = ease (percentBetweenPoints);

		Vector3 newPos = Vector3.Lerp (movePoints [fromPoint], movePoints [toPoint], easedPercentBetweenWaypoints);

		if (percentBetweenPoints >= 1) {
			percentBetweenPoints = 0;
			fromPoint ++;

			if (!cyclic) {
				if (fromPoint >= movePoints.Count - 1) {
					fromPoint = 0;
					movePoints.Reverse ();
				}
			}
			nextMoveTime = Time.time + waitTime;
		}

		return newPos - transform.position;
	}

	void getRotation() {
		switch (mode) {
		case "Standstill":
			transform.rotation = Quaternion.LookRotation (Vector3.forward, lookPoint);
			transform.Rotate (new Vector3 (0, 0, rotationFix));
			break;
		case "Points":
			transform.rotation = Quaternion.LookRotation (Vector3.forward, lookPoint);
			transform.Rotate (new Vector3 (0, 0, rotationFix));
			break;
		case "Wander":
			break;
		}
	}

	void getActions() {
		if (weapons.Count > 0) {
			//Check for attack opportunities;
			EnemyController closestEnemy = null;
			float closestMag = 10.0f;
			foreach (EnemyController e in gm.enemies) {
				if (Mathf.Abs (e.transform.position.x - transform.position.x) < 3 && Mathf.Abs (e.transform.position.y - transform.position.y) < 3) {
					float mag = Vector3.Magnitude (new Vector3(e.transform.position.x - transform.position.x, e.transform.position.y - transform.position.y, 0));
					if (mag < closestMag) {
						closestMag = mag;
						closestEnemy = e;
					}
				}
			}
			if (closestEnemy != null) {
				if (!performingAction && onPath) {
					previousPosition = transform.position;
				}
				performingAction = true;
				if (closestMag < flightDistance) {
					actionPoint = closestEnemy.transform.position + 2 * (transform.position - closestEnemy.transform.position);
					Vector3 direction = Vector3.ClampMagnitude(actionPoint * 1000, moveSpeed) * Time.deltaTime;
					transform.position += direction;
					return;
				} else {
					actionPoint = closestEnemy.transform.position;
					if (weapons [currentWeapon].currentLoaded > 0) {
						if (actionTimer >= actionDelay) {
							fireWeaponAt (actionPoint);
						} else {
							actionTimer += Time.deltaTime;
						}
					} else {
						reloading = true;
					}
				}
			} else {
				if (performingAction) {
					if (Mathf.Abs (previousPosition.x - transform.position.x) > .001 || Mathf.Abs (previousPosition.y - transform.position.y) > .001) {
						positionFix = true;
						onPath = false;
					}
				}
				performingAction = false;
				actionTimer = 0.0f;
			}
		}
	}

	float ease(float x) {
		float a = easeAmount + 1;
		return Mathf.Pow(x,a) / (Mathf.Pow(x,a) + Mathf.Pow(1-x,a));
	}
}
