using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	public GameManager gm;
	public Texture2D combatCursor;
	public Texture2D commandCursor;
	public Texture2D buildCursor;
	public string activeCursor = "Combat";
	public bool ccEnabled = false; 

	void Start() 
	{ 
		//Call the 'SetCustomCursor' (see below) with a delay of 2 seconds.  
		Invoke("SetCustomCursor", 0.5f); 
	} 

	public void GMUpdate() {
		if (gm.playerMode != activeCursor) {
			SetCustomCursor ();
		}
		getCameraMovement ();
	}

	void getCameraMovement() {
		return;
	}

	void OnDisable()  
	{ 
		//Resets the cursor to the default 
		Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);  
		this.ccEnabled = false; 
	} 

	private void SetCustomCursor() 
	{   
		switch (gm.playerMode) {
		case "Combat":
			Cursor.SetCursor (combatCursor, new Vector2 (-0.5f, 0.5f), CursorMode.Auto); 
			break;
		case "Command":
			Cursor.SetCursor (commandCursor, Vector2.zero, CursorMode.Auto);
			break;
		case "Build":
			Cursor.SetCursor (buildCursor, Vector2.zero, CursorMode.Auto);
			break;
		}
		activeCursor = gm.playerMode;
		this.ccEnabled = true; 
	} 
}
