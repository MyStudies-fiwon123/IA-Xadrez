using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
	public static Board instance;
	public Dictionary<Vector2Int, Tile> tiles = new Dictionary<Vector2Int, Tile>();

	public List<Piece> goldPieces = new List<Piece>();
	public List<Piece> greenPieces = new List<Piece>();

	private void Awake() {
		instance = this;
		CreateBoard();
	}


	public void AddPiece(string team, Piece piece) {
		Vector2 v2Pos = piece.transform.position;
		Vector2Int pos = new Vector2Int((int)v2Pos.x, (int)v2Pos.y);
		piece.tile = tiles[pos];
		piece.tile.content = piece;

		if(team == "GoldPieces") {
			goldPieces.Add(piece);
		} else {
			greenPieces.Add(piece);
		}
	}

	void CreateBoard() {
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
