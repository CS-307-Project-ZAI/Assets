using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributesZ : Attributes {
    //attributes for herding behavior. The smarter the entity, the lower the attributes.
    [Range(0, 10)]    public int boidsA = 5; //attraction radius
    [Range(0, 10)]    public int boidsO = 5; //orientation radius
    [Range(0, 10)]    public int boidsR = 5; //repulsion radius

    public TriggersAttributes herding;
    public List<EnemyController> herd;
    
	// Use this for initialization
	void Start () {
		herding = (TriggersAttributes)Instantiate(owner.gm.triggerAttribute);
    }
	
	// Update is called once per frame
	void Update () {
        this.transform.position = owner.transform.position;
    }

    public void setAttributes(int difficulty)
    {
        //add set attribute function of difficulty

        inSight.zone.radius = sight * 10f;
        inHearing.zone.radius = hearing * 6f;
        inReact.zone.radius = (1f / (reactZone + 0.1f) + 1f) * 0.24f;

        herding.zone.radius = sight * 10f;
    }
}
