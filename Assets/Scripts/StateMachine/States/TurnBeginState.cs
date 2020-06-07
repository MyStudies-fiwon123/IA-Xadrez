using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TurnBeginState : State
{
	public override async void Enter() {
		Debug.Log("Turn Begin:");
		if (machine.curentlyPlaying == machine.player1)
			machine.curentlyPlaying = machine.player2;
		else
			machine.curentlyPlaying = machine.player1;

		Debug.Log(machine.curentlyPlaying+" now playing");
		await Task.Delay(100);
		machine.ChangeTo<PieceSelectionState>();
	}
}
