using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LoadState : State
{
	public override async void Enter() {
		Debug.Log("Ola");
		await Board.instance.Load();
		await LoadAllPiecesAsync();
		machine.curentlyPlaying = machine.player2;
		machine.ChangeTo<TurnBeginState>();
	}
	async Task LoadAllPiecesAsync() {
		LoadTeamPieces(Board.instance.greenPieces);
		LoadTeamPieces(Board.instance.goldPieces);
		await Task.Delay(100);
	}

	private void LoadTeamPieces(List<Piece> pieces) {
		foreach (Piece p in pieces) {
			Board.instance.AddPiece(p.transform.parent.name, p);
		}
	}
}
