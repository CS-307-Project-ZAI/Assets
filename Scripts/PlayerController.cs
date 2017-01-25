using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float moveSpeed = 0.2f;
	public float rotation = 45.0f;
	public Bullet bullet;

	[HideInInspector]
	//Rigidbody2D rb2d;
	float angle;
	float mouseX, mouseY;
	public float bulletSpeed = 4.0f;

	// Use this for initialization
	void Start () {
		//rb2d = GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		getMovement ();
		getRotation ();
		getActions ();
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
	}

	void getActions() {
		if (Input.GetMouseButtonDown (0)) {
			fireBullet ();
		}
	}

	void fireBullet() {
		Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		float dirX = mousePos.x - transform.position.x;
		float dirY = mousePos.y - transform.position.y;
		Vector3 dir = new Vector3(dirX, dirY, 0);
		if (Vector3.Magnitude (dir) < .01) {
			return;
		}
		Bullet b = (Bullet) Instantiate (bullet);
		b.player = this;
		b.transform.position = transform.position;
		b.direction = Vector3.ClampMagnitude (dir * 1000, 1.0f) * bulletSpeed;
		b.transform.rotation = transform.rotation;
	}
}
