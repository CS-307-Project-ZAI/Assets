using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	public Texture2D combatCursor;
	public Texture2D commandCursor;
	public Texture2D buildCursor;

	[HideInInspector]
	public bool ccEnabled = false; 
	GameManager gm;
	string activeCursor = "Combat";

	void Start() 
	{ 
		gm = FindObjectOfType<GameManager> ();
		SetCustomCursor ();
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

	public void SetCustomCursor() 
	{   
		if (gm.paused) {
			Cursor.SetCursor (null, Vector2.zero, CursorMode.Auto);
			activeCursor = "Default";
		} else {
			switch (gm.playerMode) {
			case "Combat":
				Cursor.SetCursor (combatCursor, new Vector2 (combatCursor.width / 2.0f, combatCursor.height / 2.0f), CursorMode.Auto); 
				break;
			case "Command":
				Cursor.SetCursor (commandCursor, Vector2.zero, CursorMode.Auto);
				break;
			case "Build":
				Cursor.SetCursor (buildCursor, Vector2.zero, CursorMode.Auto);
				break;
			}
			activeCursor = gm.playerMode;
		}
		this.ccEnabled = true; 
	} 
}
