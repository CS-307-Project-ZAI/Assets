using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggersAttributes : MonoBehaviour
{
	public Attributes parentAttr;
	public float radius;
	public CircleCollider2D zone;
	public List<PersonController> withinRange;
	//  public List<GameObject> withinRange;

	// Use this for initialization
	void Awake()
	{
		zone = this.gameObject.GetComponent<CircleCollider2D>();
	}

	//private void OnTriggerEnter2D(Collider2D other)
	//{
	//	if (other.gameObject.tag == "Player" || other.gameObject.tag == "Ally" || other.gameObject.tag == "Enemy")
	//	{
	//		PersonController temp = other.transform.gameObject.GetComponent<PersonController>();
	//		if (!parentAttr.proximityNPCs.Contains(temp))
	//			parentAttr.proximityNPCs.Add(temp);
	//		if (!withinRange.Contains(temp))
	//			withinRange.Add(temp);
	//	}

	//}

	//private void OnTriggerExit2D(Collider2D collision)
	//{
	//	if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Ally" || collision.gameObject.tag == "Enemy")
	//	{
	//		PersonController temp = collision.gameObject.GetComponent<PersonController>();
	//		withinRange.Remove(temp);
	//		parentAttr.RemoveProx(temp);
	//	}
	//}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player" || other.gameObject.tag == "Ally")
		{
			PersonController temp = other.transform.gameObject.GetComponent<PersonController>();
			if (!parentAttr.proximityAllies.Contains(temp))
				parentAttr.proximityAllies.Add(temp);
			if (!withinRange.Contains(temp))
				withinRange.Add(temp);
		}
		if (other.gameObject.tag == "Enemy")
		{
			EnemyController temp = other.transform.gameObject.GetComponent<EnemyController>();
			if (!parentAttr.proximityEnemies.Contains(temp))
				parentAttr.proximityEnemies.Add(temp);
			if (!withinRange.Contains(temp))
				withinRange.Add(temp);
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Ally")
		{
			PersonController temp = collision.gameObject.GetComponent<PersonController>();
			withinRange.Remove(temp);
			parentAttr.RemoveAlly(temp);
			return;
		}
		if (collision.gameObject.tag == "Enemy")
		{
			PersonController temp = collision.gameObject.GetComponent<PersonController>();
			withinRange.Remove(temp);
			parentAttr.RemoveEnemy(temp);
			return;
		}
	}

	// Update is called once per frame
	public void GMUpdate()
	{
		this.transform.position = parentAttr.transform.position;
		for (int i = 0; i < withinRange.Count; i++)
			if (withinRange[i] == null)
				withinRange.RemoveAt(i);
	}

	public void setRadius(float radius)
	{
		this.radius = radius;
		zone.radius = radius;
	}

	public void setParent(Attributes par)
	{
		parentAttr = par;
	}

	public bool inRange(PersonController npc)
	{
		return withinRange.Contains(npc);
	}

	public int inRangeNb()
	{
		return withinRange.Count;
	}
}