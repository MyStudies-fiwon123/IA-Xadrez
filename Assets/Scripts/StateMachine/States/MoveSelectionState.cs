using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSelectionState : State
{
	public override void Enter() {
		Debug.Log("MoveSelectionState");
		List<Tile> moves = Board.instance.selectedPiece.movement.GetValidMoves();
		Highlights.instance.SelectTiles(moves);
        Board.instance.tileClicked += OnHighlightClicked;
	}
    public override void Exit(){
        Highlights.instance.DeSelectTiles();
        Board.instance.tileClicked -= OnHighlightClicked;
    }

    private void OnHighlightClicked(object sender, object args)
    {
        HighlightClick highlight = sender as HighlightClick;
        if (highlight == null)
            return;
        Vector3 v3Pos = highlight.transform.position;
        Vector2Int pos = new Vector2Int((int)v3Pos.x, (int)v3Pos.y);
        Tile tileClicked = highlight.tile;
        Debug.Log(tileClicked.pos);
        Board.instance.selectedHighlight = highlight;
        machine.ChangeTo<PieceMovementState>();
    }
}
