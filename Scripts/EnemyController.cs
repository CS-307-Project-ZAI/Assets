using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : PersonController {

	public PersonController target;
    public float attackRate = 1.0f;
	public int damage = 2;

    public int spawnID;
    string enemyType;

	public Dictionary<string, int> enemyInventory;
	public Item itemCloth;
	public Item itemWood;
	public Item itemMetal;

	public AttackCollider ac;

	public AttributesZ stats = null;

	// Use this for initialization
	new void Start () {
        enemyType = EnemyType.getType(spawnID);
		attackTimer = 0.0f;
		//gm = FindObjectOfType<GameManager> ();
		//PathRequestManager.RequestPath(this, transform.position, target.transform.position, OnPathFound);
		pathFindTimer = pathRefreshTime;

		AttackCollider acLoad = Resources.Load ("Attributes/AttackCollider", typeof(AttackCollider)) as AttackCollider;
		ac = (AttackCollider) Instantiate (acLoad);
		ac.setRadius (transform.GetComponent<CircleCollider2D>().radius);
		ac.owner = this;
		ac.transform.parent = transform;
		ac.transform.position = transform.position;

		stats = (AttributesZ)Instantiate(gm.AttributeZ);
		stats.setOwner(this);
		stats.mode = "Idle";

		//Add items to enemy Inventory
		enemyInventory = new Dictionary<string,int> ();
		enemyInventory.Add ("cloth", 0);
		enemyInventory.Add ("wood", 0);
		enemyInventory.Add ("metal", 0);
		int clothNum = (int)Random.Range (0.0f, 3.0f);
		int woodNum = (int)Random.Range (0.0f, 2.0f);
		int metalNum = (int)Random.Range (0.0f, 2.0f);
		enemyInventory ["cloth"] = clothNum;
		enemyInventory ["wood"] = woodNum;
		enemyInventory ["metal"] = metalNum;
		//Debug.Log("EnemyInventory: " + "cloth: " + enemyInventory ["cloth"] + " | wood: " + enemyInventory ["wood"] + " | metal: " + enemyInventory ["metal"]);
		itemCloth = Resources.Load ("Materials/cloth", typeof(Item)) as Item;
		itemWood = Resources.Load ("Materials/wood", typeof(Item)) as Item;
		itemMetal = Resources.Load ("Materials/metal", typeof(Item)) as Item;
	}
	
	// Update is called once per frame
	public void GMUpdate () {
		if (stats == null) {
			return;
		}
		if (kill) {
			//Drop items in enemy inventory here
			int clothDropNum = this.enemyInventory["cloth"];
			int woodDropNum = this.enemyInventory["wood"];
			int metalDropNum = this.enemyInventory["metal"];
			if (clothDropNum == 2) {
				Item dropItemCloth = (Item)Instantiate (itemCloth, new Vector3 (transform.position.x + .3f, transform.position.y, 0), Quaternion.identity);
				Item dropItemCloth2 = (Item)Instantiate (itemCloth, new Vector3 (transform.position.x, transform.position.y + .3f, 0), Quaternion.identity);
				dropItemCloth.gm = this.gm;
				dropItemCloth2.gm = this.gm;
			} else if (clothDropNum == 1) {
				Item dropItemCloth = (Item)Instantiate (itemCloth, new Vector3 (transform.position.x, transform.position.y + .3f, 0), Quaternion.identity);
				dropItemCloth.gm = this.gm;
			}
			if (woodDropNum == 1) {
				Item dropItemWood = (Item)Instantiate (itemWood, new Vector3 (transform.position.x - .3f, transform.position.y, 0), Quaternion.identity);
				dropItemWood.gm = this.gm;
			}
			if (metalDropNum == 1) {
				Item dropItemMetal = (Item)Instantiate (itemMetal, new Vector3 (transform.position.x, transform.position.y - .3f, 0), Quaternion.identity);
				dropItemMetal.gm = this.gm;
			}
			return;
		}
        attackTimer += Time.deltaTime;
		if (stats != null) {
			stats.GMUpdate ();
		}
		getMovement ();
		getRotation ();
		if (targetTag != null) {
			targetTag.transform.rotation = Quaternion.identity;
			targetTag.transform.position = new Vector3 (transform.position.x, transform.position.y + 0.5f, 0);
		}
	}

	void getMovement() {
		if (stats.mode == "Offensive") {
			target = stats.proximityAllies[0];
			pathFindTimer += (pathFindTimer >= pathRefreshTime ? 0.0f : Time.deltaTime);
			if (euclideanDistance(transform.position, target.transform.position) > 10.0f) {
				//If distance is large enough, perform less resource-intensive pathfinding
				StopCoroutine ("FollowPath");
				float dirX = target.transform.position.x - transform.position.x;
				float dirY = target.transform.position.y - transform.position.y;
				Vector3 dir = new Vector3 (dirX, dirY, 0);
				transform.position += Vector3.ClampMagnitude (dir, moveSpeed) * Time.deltaTime;
			} else if (pathFindTimer >= pathRefreshTime) {
				//Otherwise, if our timer is greater than our path refresh time, we'll perform A* for this unit
				pathFindTimer = 0.0f;
				PathRequestManager.RequestPath(this, transform.position, target.transform.position, OnPathFound);
			}
		}
		else {
			if(stats.mode == "Herding")
			{
				Vector3 direction = Vector3.ClampMagnitude((stats.influenceOfNPCs + stats.movement) * 250, 0.2f * stats.speed * Time.deltaTime);
				transform.position += direction;
				stats.movement = direction;
				//this.transform.forward = direction;
				return;
			}
			if(stats.mode == "Idle")
			{
				Vector3 direction = Vector3.ClampMagnitude(stats.movement * 500, 0.2f * stats.speed) * Time.deltaTime;
				transform.position += direction;
				stats.movement = direction;
				return;
			}
		}
	}

	void getRotation() {
		if (target == null)
		{
			transform.rotation = Quaternion.LookRotation(Vector3.forward, stats.movement);
		}
		else
		{
			if (path == null)
			{
				transform.rotation = Quaternion.LookRotation(Vector3.forward, target.transform.position - transform.position);
			}
			else if (path.Length > 0)
			{
				transform.rotation = Quaternion.LookRotation(Vector3.forward, path[targetIndex] - transform.position);
			}
			else
			{
				transform.rotation = Quaternion.LookRotation(Vector3.forward, target.transform.position - transform.position);
			}
		}
		transform.Rotate (new Vector3 (0, 0, this.rotationFix));
	}
}

public static class EnemyType {
    //add new types here
    private static string[] types = {"zombie"};

    public static string getType(int id) {
        if (id >= 0 && id < types.Length) return types[id];
        return "unknown";
    }

    public static int getTypeSize() {
        return types.Length;
    }
}