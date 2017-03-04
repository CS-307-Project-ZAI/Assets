using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : PersonController {

	public PlayerController target;
    public float attackRate = 1.0f;
	public int damage = 2;

	// Use this for initialization
	new void Start () {
		attackTimer = 0.0f;
		//gm = FindObjectOfType<GameManager> ();
		//PathRequestManager.RequestPath(this, transform.position, target.transform.position, OnPathFound);
	}
	
	// Update is called once per frame
	public void GMUpdate () {
        attackTimer += Time.deltaTime;
		getMovement ();
		getRotation ();
		if (targetTag != null) {
			targetTag.transform.rotation = Quaternion.identity;
			targetTag.transform.position = new Vector3 (transform.position.x, transform.position.y + 0.5f, 0);
		}
	}

	void getMovement() {
		pathFindTimer += (pathFindTimer >= pathRefreshTime ? 0.0f : Time.deltaTime);
		/**
		if (euclideanDistance(transform.position, target.transform.position) > 5.0f) {
			//If distance is large enough, perform less resource-intensive pathfinding
			StopCoroutine ("FollowPath");
			float dirX = target.transform.position.x - transform.position.x;
			float dirY = target.transform.position.y - transform.position.y;
			Vector3 dir = new Vector3 (dirX, dirY, 0);
			transform.position += Vector3.ClampMagnitude (dir, moveSpeed) * Time.deltaTime;
		} else 
		*/
		if (pathFindTimer >= pathRefreshTime) {
			//Otherwise, if our timer is greater than our path refresh time, we'll perform A* for this unit
			pathFindTimer = 0.0f;
			PathRequestManager.RequestPath (this, transform.position, target.transform.position, OnPathFound);
		}
	}

	void getRotation() {
		if (path == null) {
			transform.rotation = Quaternion.LookRotation (Vector3.forward, target.transform.position - transform.position);
		} else if (path.Length > 0) {
			transform.rotation = Quaternion.LookRotation (Vector3.forward, path[targetIndex] - transform.position);
		} else {
			transform.rotation = Quaternion.LookRotation (Vector3.forward, target.transform.position - transform.position);
		}
		transform.Rotate (new Vector3 (0, 0, this.rotationFix));
	}

	

	void OnTriggerStay2D(Collider2D col) {
		if (col.gameObject.tag == "Player") {
			if (attackTimer >= attackRate) {
				col.gameObject.SendMessage ("ApplyDamage", damage);
				attackTimer = 0.0f;
			}
		} else if (col.gameObject.tag == "Ally") {
			if (attackTimer >= attackRate) {
				col.gameObject.SendMessage ("ApplyDamage", damage);
				attackTimer = 0.0f;
			}
		} else if (col.gameObject.tag == "Wall") {
			if (attackTimer >= attackRate) {
				col.gameObject.SendMessage ("ApplyDamage", damage);
				attackTimer = 0.0f;
			}
		}
	}
}
