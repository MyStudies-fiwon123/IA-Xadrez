using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TurnEndState : State
{
	public override async void Enter() {
		Debug.Log("Em TurnEndState: ");
		bool gameFinished = CheckTeams();
		await Task.Delay(100);
		if (gameFinished)
			machine.ChangeTo<GameEndState>();
		else
			machine.ChangeTo<TurnBeginState>();
	}

	private bool CheckTeams() {
		Piece goldPiece = Board.instance.goldPieces.Find((x) => x.gameObject.activeSelf == true);
		if (goldPiece == null) {
			Debug.Log("Lado verde ganhou");
			return true;
		}
		Piece greenPiece = Board.instance.greenPieces.Find((x) => x.gameObject.activeSelf == true);
		if (greenPiece == null) {
			Debug.Log("Lado dourado ganhou");
			return true;
		}
		return false;
	}
}
