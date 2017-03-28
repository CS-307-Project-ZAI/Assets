using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributesZ : Attributes
{
	//attributes for herding behavior. The smarter the entity, the lower the attributes.
	[Range(0, 10)]
	public int boidsA = 5; //attraction radius
	[Range(0, 10)]
	public int boidsO = 5; //orientation radius
	[Range(0, 10)]
	public int boidsR = 5; //repulsion radius

	//public TriggerAttributesZ herding;
	//public List<EnemyController> herd;
	public bool isHerding = false;
	public bool AllyInRange;
	public AllyController closestAlly = null;
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

		//herding = (TriggerAttributesZ)Instantiate(owner.gm.triggerAttributeZ);
		//herding.transform.SetParent(this.transform);
		//herding.setParent(this);

		this.transform.SetParent(owner.transform);
		setAttributes(owner.gm.difficulty);
	}

	// Update is called once per frame
	void Update()
	{
		this.transform.position = owner.transform.position;
		for (int i = 0; i < proximityNPCs.Count; i++)
		{
			if (proximityNPCs[i] == null)
				proximityNPCs.RemoveAt(i);
			if (proximityNPCs[i].gameObject.tag == "Player" || proximityNPCs[i].gameObject.tag == "Ally")
				enemyInRange = true;
		}
		getInfluences(Time.deltaTime);
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
				getInfluenceEnemies();
			}
			else
			{
				mode = "Idle";
			}
		}
	}

	new public void getClosestFoe()
	{
		float shortestDistance = 1000000;
		AllyController closest = null;
		for (int i = 0; i < proximityAllies.Count; i++)
		{
			if (Vector3.Dot(this.transform.position - proximityAllies[i].transform.position, owner.transform.forward) > blindspot)
			{
				float distance = (this.transform.position - proximityAllies[i].transform.position).sqrMagnitude;
				if (distance < shortestDistance)
				{
					shortestDistance = distance;
					closest = (AllyController)proximityAllies[i];
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
		Vector3 repulsion = -averageDirectionWeighted(inReact.withinRange) * 5.0f;

		Vector3 orientation = Vector3.zero;
		foreach(EnemyController e in inHearing.withinRange)
		{
			orientation += e.transform.forward;
		}
		orientation.Normalize();
		orientation = orientation * 2.5f;

		Vector3 attraction = averageDirectionWeighted(inSight.withinRange) * 1.0f;

		Vector3 sum = repulsion + orientation + attraction;
		sum.Normalize();

		influenceOfNPCs = sum; 
	}
}
