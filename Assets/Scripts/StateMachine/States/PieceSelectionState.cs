using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceSelectionState : State
{
	public override void Enter() {
		Board.instance.tileClicked += PieceClicked;
	}

	public override void Exit() {
		Board.instance.tileClicked -= PieceClicked;
	}

	private void PieceClicked(object sender, object args) {
		Piece piece = sender as Piece;
		Player player = args as Player;
		if (machine.curentlyPlaying == player) {
			Debug.Log(piece + " was clicked");
			Board.instance.selectedPiece = piece;
			machine.ChangeTo<MoveSelectionState>();
		}

	}
}
