using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour {
	public GameManager gm;
	public Texture2D combatCursor;
	public Texture2D commandCursor;
	public Texture2D buildCursor;
	public CursorMode cursorMode = CursorMode.Auto;
	public Vector2 hotSpot = Vector2.zero;

	void OnMouseEnter() {
		Cursor.SetCursor(combatCursor, hotSpot, cursorMode);
	}

	void OnMouseExit() {
		Cursor.SetCursor(null, Vector2.zero, cursorMode);
	}
}
