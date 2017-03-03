using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attributes : MonoBehaviour {
    public PersonController owner;

    //Attributes common to all NPCs
    public int health = 50;
    [Range(0, 10)]    public int intelligence = 5;
    [Range(0, 10)]    public int sight = 5;
    [Range(0, 10)]    public int hearing = 5;
    [Range(0, 10)]    public int reactZone = 5; //agression or panic trigger zone
    [Range(-1, 1)]    public float blindspot = -0.1f; // -1 = 360 view, 1 = totally blind
    [Range(0, 10)]    public int speed = 5;
    [Range(0, 10)]    public int strength = 5;
    public float attackRate = 1.0f;
    
    public List<PersonController> proximityNPCs;

    [HideInInspector]   public TriggersAttributes inSight;
    [HideInInspector]    public TriggersAttributes inHearing;
    [HideInInspector]    public TriggersAttributes inReact;
    [HideInInspector]    public Vector3 influenceOfNPCs = Vector3.zero;

	// Use this for initialization
	void Start () {
        inSight = (TriggersAttributes)Instantiate(owner.gm.triggerAttribute);
        inHearing = (TriggersAttributes)Instantiate(owner.gm.triggerAttribute);
        inReact = (TriggersAttributes)Instantiate(owner.gm.triggerAttribute);
    }
	
	// Update is called once per frame
	void Update () {
        this.transform.position = owner.transform.position;
        
	}

    // Set attributes for ally NPCs
    public void setAttributes()
    {
        //add setup function

        inSight.zone.radius = sight * 10f;
        inHearing.zone.radius = hearing * 6f;
        inReact.zone.radius = (1f / (reactZone + 0.1f) + 1f) * 0.24f;
    }

    //call to remove an NPC from proximity list
    //checks that the npc has been removed from all 3 senses triggers
    public void RemoveProx(PersonController npc)
    {
        if(!inHearing.withinRange.Contains(npc) && !inSight.withinRange.Contains(npc) && !inReact.withinRange.Contains(npc))
        {
            proximityNPCs.Remove(npc);
        }

    }
}
