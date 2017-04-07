using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributesZ : Attributes
{
	//attributes for herding behavior. The smarter the entity, the lower the attributes.
	[Range(0, 10)]
	public float boidsA = 5f; //attraction radius
	[Range(0, 10)]
	public float boidsO = 3f; //orientation radius
	[Range(0, 10)]
	public float boidsR = 0.5f; //repulsion radius

	//public TriggerAttributesZ herding;
	//public List<EnemyController> herd;
	public bool isHerding = false;
	public bool AllyInRange;
	public PersonController closestAlly = null;
	// Use this for initialization
	void Start()
	{
		inSight = (TriggersAttributes)Instantiate(owner.gm.triggerAttribute);
		inHearing = (TriggersAttributes)Instantiate(owner.gm.triggerAttribute);
		inReact = (TriggersAttributes)Instantiate(owner.gm.triggerAttribute);
		inSight.transform.SetParent(this.transform);
		inHearing.transform.SetParent(this.transform);
		inReact.transform.SetParent(this.transform);
		inReact.setParent(this);
		inHearing.setParent(this);
		inSight.setParent(this);
		//inReact.setRadius(0.6f);
		//herding = (TriggerAttributesZ)Instantiate(owner.gm.triggerAttributeZ);
		//herding.transform.SetParent(this.transform);
		//herding.setParent(this);

		this.transform.SetParent(owner.transform);
		setAttributes(ApplicationModel.difficulty);
		start = true;
	}

	// Update is called once per frame
	public void GMUpdate()
	{
		if (start) {
			inSight.GMUpdate ();
			inHearing.GMUpdate ();
			inReact.GMUpdate ();
			this.transform.position = owner.transform.position;
			for (int i = 0; i < proximityNPCs.Count; i++) {
				if (proximityNPCs [i] == null)
					proximityNPCs.RemoveAt (i);
				if (proximityNPCs [i].gameObject.tag == "Player" || proximityNPCs [i].gameObject.tag == "Ally")
					enemyInRange = true;
			}
			getInfluences (Time.deltaTime);
		}
	}

	new public void setAttributes(string difficulty)
	{
		//add set attribute function of difficulty

		inSight.setParent(this);
		inSight.setRadius(sight * 0.95f);
		inHearing.setParent(this);
		inHearing.setRadius(hearing * 0.70f);
		inReact.setParent(this);
		inReact.setRadius((10f - reactZone) * 0.24f);
		inReact.setRadius(0.6f);
	}

	new public void getInfluences(float DeltaT)
	{
		enemyInRange = proximityEnemies.Count != 0;
		AllyInRange = proximityAllies.Count != 0;

		if (AllyInRange)
		{
			getClosestFoe();
			mode = "Offensive";
			getInfluencesAlly();
		}
		else
		{
			if (enemyInRange)
			{
				mode = "Herding";
				movement += influenceOfNPCs;
				movement /= 2;
				getInfluenceEnemies();
			}
			else
			{
				mode = "Idle";
				influenceOfNPCs = movement;
			}
		}
	}

	new public void getClosestFoe()
	{
		float shortestDistance = 1000000;
		PersonController closest = null;
		for (int i = 0; i < proximityAllies.Count; i++)
		{
			if (Vector3.Dot(this.transform.position - proximityAllies[i].transform.position, owner.transform.forward) > blindspot)
			{
				float distance = (this.transform.position - proximityAllies[i].transform.position).sqrMagnitude;
				if (distance < shortestDistance)
				{
					shortestDistance = distance;
					closest = proximityAllies[i];
				}
			}
		}
		closestAlly = closest;
	}

	public void getInfluencesAlly()
	{
		Vector3 avgDirection = averageDirectionWeighted(proximityAllies);
		influenceOfNPCs = -avgDirection;
	}

	public void getInfluenceEnemies()
	{
		Vector3 sum = Vector3.zero;
		Vector3 repulsion = Vector3.zero;
		Vector3 orientation = Vector3.zero;
		Vector3 attraction = Vector3.zero;

		float BR = boidsR * boidsR;
		float BO = boidsO * boidsO;
		float BA = boidsA * boidsA;
		//foreach (PersonController e in proximityEnemies)
		//{
		//	Vector3 vect = e.transform.position - this.transform.position;
		//	float d = vect.sqrMagnitude;
		//	print(d);
		//	if (d < BR)
		//		repulsion -= vect / d;
		//	else
		//	{
		//		if (d < BO)
		//			orientation += e.transform.forward / d;
		//		else
		//		{
		//			attraction += vect / d;
		//		}
		//	}
		//}

		Vector3 vect;
		float d;
		foreach (PersonController e in inReact.withinRange)
		{
			vect = e.transform.position - this.transform.position;
			d = vect.magnitude;
			repulsion -= vect / d;
		}
		repulsion.Normalize();

		foreach (PersonController e in inHearing.withinRange)
		{
			vect = e.transform.position - this.transform.position;
			d = vect.magnitude;
			orientation += e.transform.forward / d;
		}
		orientation.Normalize();

		foreach (PersonController e in inReact.withinRange)
		{
			vect = e.transform.position - this.transform.position;
			d = vect.magnitude;
			attraction += vect / d;
		}
		attraction.Normalize();


		//Vector3 repulsion = -averageDirectionWeighted(inReact.withinRange) * 2.0f;

		//Vector3 orientation = Vector3.zero;
		//foreach(EnemyController e in inHearing.withinRange)
		//{
		//	orientation += e.transform.forward;
		//}
		//orientation.Normalize();
		//orientation = orientation * 1.5f;

		//Vector3 attraction = averageDirectionWeighted(inSight.withinRange) * 1.0f;

		repulsion.Normalize();
		orientation.Normalize();
		attraction.Normalize();

		sum = repulsion * 3 + orientation * 2 + attraction;
		sum.Normalize();
		influenceOfNPCs = sum; 
	}
}
