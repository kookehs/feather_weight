﻿using UnityEngine;
using System.Collections;

public class CollectionCursor : MonoBehaviour {

	public Texture2D cursorDefault;
	public Texture2D cursorHover;
	public Texture2D cursorHold;
	public Texture2D cursorBattle;
	public Texture2D cursorConfirm;

    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

	void Start(){
		Cursor.SetCursor(cursorDefault, hotSpot, cursorMode);
	}

    public void SetHover() {
		Cursor.SetCursor(cursorHover, hotSpot, cursorMode);
    }

    public void SetDefault() {
		Cursor.SetCursor(cursorDefault, hotSpot, cursorMode);
    }

	public void SetNone() {
		Cursor.SetCursor(null, hotSpot, cursorMode);
	}

	public void SetHold(){
		Cursor.SetCursor(cursorHold, hotSpot, cursorMode);
	}

	public void SetWeapon(){
		Cursor.SetCursor(cursorBattle, hotSpot, cursorMode);
	}

	public void SetConfirm(){
		Cursor.SetCursor(cursorConfirm, hotSpot, cursorMode);
	}

	void OnDestroy(){
		Cursor.SetCursor(null, hotSpot, cursorMode);
	}
}
