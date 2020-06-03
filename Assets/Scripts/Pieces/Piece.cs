using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Piece : MonoBehaviour
{
	public Tile tile;

	void OnMouseDown() {
		Debug.Log("Clickou em: " + transform);
	}

	private void Start() {
		Board.instance.AddPiece(transform.parent.name, this);
	}
}
