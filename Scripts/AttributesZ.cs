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
			//aggression behavior
		}
		else
		{
			if (enemyInRange)
			{
				//Boid behavior function
			}
			else
			{
				//idle behavior
			}
		}


		//if (enemyInRange)
		//{
		//	if (inReact.withinRange.Count != 0)
		//	{
		//		Panicked = true;
		//		PanickTimer = 5.0f;
		//	}

		//	getClosestFoe();

		//	if (!Panicked)
		//	{
		//		//attack closest
		//	}
		//	else
		//	{
		//		//for all enemies in proximityNPCs add direction /sqrmagnitude to influence
		//		PanickTimer -= DeltaT;
		//		Vector3 avgDirection = averageDirectionWeighted(proximityEnemies);
		//		influenceOfNPCs = -avgDirection;
		//	}
		//}
		//else
		//{
		//	//whatever idle is
		//}
	}
}
