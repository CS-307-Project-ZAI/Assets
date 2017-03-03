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
    public Wall wall;

	protected Vector3[] path = null;
	protected int targetIndex;
	public PathRequestManager.PathRequest request = null;

	[HideInInspector]
	public bool reloading = false;
	public bool kill = false;
	public GameObject targetTag = null;
	public float pathFindTimer = 0.0f;
	public float pathRefreshTime = 0.0001f;

	public int currentWeapon = 0;
	protected float attackTimer = 0.0f;
	protected bool performingAction = false;

	protected void Start() {
		//Give person starting weapon
		Weapon w = (Weapon)Instantiate (gm.startingWeapon);
		w.owner = this;
		string load = "AmmoTypes/" + w.ammoType;
		w.bullet = Resources.Load (load, typeof(Bullet)) as Bullet;
		weapons.Add (w);
		w.transform.parent = gameObject.transform;
        wall = Resources.Load("Walls/Tier1Wall", typeof(Wall)) as Wall;
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

	public void OnPathFound(Vector3[] newPath, bool pathSuccessful) {
		if (pathSuccessful) {
			path = newPath;
			targetIndex = 0;
			StopCoroutine("FollowPath");
			StartCoroutine("FollowPath");
		}
	}

	IEnumerator FollowPath() {
		if (path.Length > 0) {
			targetIndex = 0;
			Vector3 currentWaypoint = path [0];
			while (true) {
				//if (performingAction) {
				//	yield return null;
				//}
				if ((Vector3)transform.position == currentWaypoint) {
					targetIndex++;
					if (targetIndex >= path.Length) {
						targetIndex = 0;
						path = null;
						yield break;
					}
					currentWaypoint = path [targetIndex];
				}

				transform.position = Vector3.MoveTowards (transform.position, currentWaypoint, moveSpeed * Time.deltaTime);
				yield return null;
			}
		}
	}

	public void OnDrawGizmos() {
		if (path != null) {
			for (int i = targetIndex; i < path.Length; i ++) {
				Gizmos.color = Color.black;
				//Gizmos.DrawCube((Vector3)path[i], Vector3.one *.5f);

				if (i == targetIndex) {
					Gizmos.DrawLine(transform.position, path[i]);
				}
				else {
					Gizmos.DrawLine(path[i-1],path[i]);
				}
			}
		}
	}

	public float euclideanDistance(Vector3 pos, Vector3 target) {
		return Mathf.Sqrt (Mathf.Pow(target.x - pos.x, 2) + Mathf.Pow(target.y - pos.y, 2));
	}
}
