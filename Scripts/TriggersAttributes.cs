using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggersAttributes : MonoBehaviour {
	public Attributes parent;
	public int radius;
	public CircleCollider2D zone;
	public List<PersonController> withinRange;
	//  public List<GameObject> withinRange;

	// Use this for initialization
	void Start () {
		zone = (CircleCollider2D) this.transform.GetComponent("CircleCollider2D");
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player" || other.gameObject.tag == "Ally" || other.gameObject.tag == "Enemy")
		{
			PersonController temp = other.gameObject.GetComponent<PersonController>();
			if (!parent.proximityNPCs.Contains(temp))
				parent.proximityNPCs.Add(temp);
		}

	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Ally" || collision.gameObject.tag == "Enemy")
		{
			PersonController temp = collision.gameObject.GetComponent<PersonController>();
			parent.RemoveProx(temp);
		}
	}
	// Update is called once per frame
	void Update () {
		this.transform.position = parent.transform.position;
	}
}