using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attributes : MonoBehaviour
{
	public PersonController owner;

	//Attributes common to all NPCs
	public int health = 50;
	[Range(0, 10)]
	public int intelligence = 5;
	[Range(0, 10)]
	public int sight = 5;
	[Range(0, 10)]
	public int hearing = 5;
	[Range(0, 10)]
	public int reactZone = 8; //aggression or panic trigger zone
	[Range(-1, 1)]
	public float blindspot = -0.1f; // -1 = 360 view, 1 = totally blind
	[Range(0, 10)]
	public int speed = 5;
	[Range(0, 10)]
	public int strength = 5;
	[Range(0, 10)]
	public int buildRate = 1;
	public float attackRate = 1.0f;



	public string mode = "Command";
	public string prevMode = "Command";
	public string aggression = "Defensive";

	[HideInInspector]
	public List<string> modes = new List<string> { "Command", "Points", "Wander" };
	public List<string> aggressions = new List<string> { "Passive", "Defensive", "Offensive" };


	public List<PersonController> proximityNPCs;
	public List<PersonController> proximityAllies;
	public List<PersonController> proximityEnemies;

	public bool enemyInRange = false;
	public EnemyController closestEnemy = null;
	public bool Panicked = false;
	public float PanickTimer = 0;
	public bool Aggro = false;

	public TriggersAttributes inSight;
	public TriggersAttributes inHearing;
	public TriggersAttributes inReact;
	public Vector3 influenceOfNPCs = Vector3.zero;
	public Vector3 movement = new Vector3(1.0f, 0.0f);
	protected bool start = false;

	// Use this for initialization
	void Start() {
		inSight = (TriggersAttributes)Instantiate(owner.gm.triggerAttribute);
		inHearing = (TriggersAttributes)Instantiate(owner.gm.triggerAttribute);
		inReact = (TriggersAttributes)Instantiate(owner.gm.triggerAttribute);
		inSight.transform.SetParent(this.transform);
		inHearing.transform.SetParent(this.transform);
		inReact.transform.SetParent(this.transform);
		inReact.setParent(this);
		inHearing.setParent(this);
		inSight.setParent(this);

		//setAttributes(owner.gm.difficulty);
		this.transform.SetParent(owner.transform);
		setAttributes(ApplicationModel.difficulty);
		start = true;
	}

	// Update is called once per frame
	public void GMUpdate() {
		if (start) {
			inSight.GMUpdate ();
			inHearing.GMUpdate ();
			inReact.GMUpdate ();
			this.transform.position = owner.transform.position;
			cleanProximity ();
			getInfluences (Time.deltaTime);
		}
	}

	protected void cleanProximity() {
		for (int i = 0; i < proximityAllies.Count; i++)
			if (proximityAllies[i] == null)
				proximityAllies.RemoveAt(i);
		for (int i = 0; i < proximityNPCs.Count; i++)
			if (proximityNPCs[i] == null)
				proximityNPCs.RemoveAt(i);
		for (int i = 0; i < proximityEnemies.Count; i++)
			if (proximityEnemies[i] == null)
				proximityEnemies.RemoveAt(i);

		//for (int i = proximityAllies.Count - 1; i > 0 ; i--)
		//	if (proximityAllies[i] == null)
		//		proximityAllies.RemoveAt(i);
		//for (int i = proximityNPCs.Count - 1; i > 0 ; i--)
		//	if (proximityNPCs[i] == null)
		//		proximityNPCs.RemoveAt(i);
		//for (int i = proximityEnemies.Count - 1; i > 0; i--)
		//	if (proximityEnemies[i] == null)
		//		proximityEnemies.RemoveAt(i);
	}

	// Set attributes for ally NPCs
	public void setAttributes(string difficulty) {
		//add setup function

		inSight.setParent(this);
		inSight.setRadius(sight * 0.95f);
		inHearing.setParent(this);
		inHearing.setRadius(hearing * 0.70f);
		inReact.setParent(this);
		inReact.setRadius((10f - reactZone) * 0.24f);
	}

	//call to remove an NPC from proximity list
	//checks that the npc has been removed from all 3 senses triggers
	public void RemoveProx(PersonController npc) {
		if (!inHearing.withinRange.Contains(npc) && !inSight.withinRange.Contains(npc) && !inReact.withinRange.Contains(npc))
		{
			proximityNPCs.Remove(npc);
		}
	}

	public void RemoveEnemy(PersonController npc) {
		if (!inHearing.withinRange.Contains(npc) && !inSight.withinRange.Contains(npc) && !inReact.withinRange.Contains(npc))
		{
			proximityEnemies.Remove(npc);
		}
	}

	public void RemoveAlly(PersonController npc) {
		if (!inHearing.withinRange.Contains(npc) && !inSight.withinRange.Contains(npc) && !inReact.withinRange.Contains(npc))
		{
			proximityAllies.Remove(npc);
		}
	}

	public void setOwner(PersonController p) {
		owner = p;
	}

	public void getClosestFoe() {
		float shortestDistance = 1000000;
		EnemyController closest = null;
		for (int i = 0; i < proximityEnemies.Count; i++)
		{
			if (Vector3.Dot(this.transform.position - proximityEnemies[i].transform.position, owner.transform.forward) > blindspot)
			{
				float distance = (this.transform.position - proximityEnemies[i].transform.position).sqrMagnitude;
				if (distance < shortestDistance)
				{
					shortestDistance = distance;
					closest = (EnemyController)proximityEnemies[i];
				}
			}
		}
		closestEnemy = closest;
	}

	protected Vector3 averageDirectionWeighted(List<PersonController> npcs) {
		Vector3 avg = Vector3.zero;

		foreach (PersonController npc in npcs)
		{
			avg += npc.transform.position - this.transform.position;
		}
		avg.Normalize();
		return avg;
	}


	protected void getInfluences(float DeltaT) {
		enemyInRange = proximityEnemies.Count != 0;

		if (enemyInRange)
		{
			if (inReact.withinRange.Count != 0)
			{
				Panicked = true;
				PanickTimer = 2.0f;
			}

			getClosestFoe();

			if (!Panicked)
			{
				//attack closest
			}
			else
			{
				//for all enemies in proximityNPCs add direction /sqrmagnitude to influence
				if(PanickTimer > 0)
						PanickTimer -= DeltaT;
				else
					Panicked = false;

				Vector3 avgDirection = averageDirectionWeighted(proximityEnemies);
				influenceOfNPCs = -avgDirection;
			}
		}
		else
		{
			if (Panicked)
			{
				if (PanickTimer > 0)
					PanickTimer -= DeltaT;
				else
					Panicked = false;

				Vector3 avgDirection = averageDirectionWeighted(proximityEnemies);
				influenceOfNPCs = -avgDirection;
			}
			//whatever idle is
		}
	}
}
