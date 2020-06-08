using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Piece : MonoBehaviour
{
	[HideInInspector]
	public Movement movement;
	public Tile tile;
    public bool wasMoved;

	void OnMouseDown() {
		InputController.instance.tileClicked(this, transform.parent.GetComponent<Player>());
	}
}
