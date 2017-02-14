using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

	public PersonController owner;
	public Bullet bullet;
	public string weaponName;
	public string type;
	public string ammoType;
	public int maxAmmo;
	public int clipSize;
	public int ammoPool;
	public float skew;
	public int damage;
	public float bulletSpeed = 10.0f;
	public float fireRate = 1.0f;
	public float reloadTime = 3.0f;
	public bool passthrough = false;

	[HideInInspector]
	public int currentLoaded;

	public float reloadTimer = 0.0f;

	// Use this for initialization
	void Start () {
		currentLoaded = clipSize;
	}
	
	// Update is called once per frame
	public void ControlledUpdate () {
		//Update transform of weapon to follow owner
		this.transform.position = owner.transform.position;
		this.transform.rotation = owner.transform.rotation;

		//Update reloading information
		if (owner.reloading) {
			reloadTimer += Time.deltaTime;
			if (reloadTimer >= reloadTime) {
				//Done reloading
				reloadTimer = 0.0f;
				int difference = clipSize - currentLoaded;
				int loadAmount = ((difference < ammoPool) ? difference : ammoPool);
				if (ammoPool != -1) {
					ammoPool -= loadAmount;
					currentLoaded += loadAmount;
				} else {
					currentLoaded = clipSize;
				}
				owner.reloading = false;
			}
		}
	}

	void fireWeapon() {
		Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		float dirX = mousePos.x - transform.position.x;
		float dirY = mousePos.y - transform.position.y;
		Vector3 dir = new Vector3 (dirX, dirY, 0);
		if (Vector3.Magnitude (dir) < .01) {
			return;
		}
		fireBullet (dir);
	}

	public void fireWeaponAt(Vector3 point) {
		float dirX = point.x - transform.position.x;
		float dirY = point.y - transform.position.y;
		Vector3 dir = new Vector3 (dirX, dirY, 0);
		if (Vector3.Magnitude (dir) < .01) {
			return;
		}
		fireBullet (dir);
	}

	void fireBullet(Vector3 dir) {
		Bullet b = (Bullet)Instantiate (bullet);
		b.owner = owner;
		b.transform.position = new Vector3 (transform.position.x, transform.position.y, 0);
		owner.gm.bullets.Add (b);
		float randomSkew = (2 * Random.value * skew) - skew;
		float angle = Vector3.Angle (Vector3.right, dir);
		if (dir.y >= 0) {
			angle += randomSkew;
		}
		else {
			angle = -angle + randomSkew;
		}
		dir = new Vector3 (Mathf.Cos (Mathf.Deg2Rad * angle), Mathf.Sin (Mathf.Deg2Rad * angle), 0);

		b.direction = Vector3.ClampMagnitude (dir * 1000, 1.0f) * bulletSpeed;
		b.transform.rotation = transform.rotation;
		b.transform.Rotate (new Vector3 (0, 0, -owner.rotationFix));
		b.damage = damage;
		b.passthrough =  passthrough;
		if (clipSize != -1) {
			currentLoaded--;
		}
	}

	void interruptReload() {
		reloadTimer = 0.0f;
	}
}
