using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Piece : MonoBehaviour
{
	[HideInInspector]
	public Movement movement;
	public Tile tile;
    public bool wasMoved;
    public bool maxTeam;
    virtual protected void Start(){
        if (transform.parent.name == "GoldPieces")
            maxTeam = true;
    }
    public virtual AffectedPiece CreateAffected(){
        return new AffectedPiece();
    }
	void OnMouseDown() {
		InputController.instance.tileClicked(this, transform.parent.GetComponent<Player>());
	}
}
