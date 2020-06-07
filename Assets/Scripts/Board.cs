using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public delegate void TileClickedEvent(object sender, object args);

public class Board : MonoBehaviour
{
	public static Board instance;
	public Dictionary<Vector2Int, Tile> tiles = new Dictionary<Vector2Int, Tile>();
	public Transform goldHolder { get { return StateMachineController.instance.player1.transform; } }
	public Transform greenHolder { get { return StateMachineController.instance.player2.transform; } }

	public List<Piece> goldPieces = new List<Piece>();
	public List<Piece> greenPieces = new List<Piece>();
	public TileClickedEvent tileClicked = delegate { };
	public Piece selectedPiece;
	public HighlightClick selectedHighlight;

	private void Awake() {
		instance = this;
	}

	public async Task Load() {
		GetTeams();

		await Task.Run(() => CreateBoard());
	}

	void GetTeams() {
		goldPieces.AddRange(goldHolder.GetComponentsInChildren<Piece>());
		greenPieces.AddRange(greenHolder.GetComponentsInChildren<Piece>());
	}

	public void AddPiece(string team, Piece piece) {
		Vector2 v2Pos = piece.transform.position;
		Vector2Int pos = new Vector2Int((int)v2Pos.x, (int)v2Pos.y);
		piece.tile = tiles[pos];
		piece.tile.content = piece;
	}

	public void CreateBoard() {
		for (int i=0; i<8; i++) {
			for (int j=0; j<8; j++) {
				CreateTile(i, j);
			}
		}
	}

	private void CreateTile(int i, int j) {
		Tile tile = new Tile();
		tile.pos = new Vector2Int(i, j);
		tiles.Add(tile.pos, tile);
	}
}
