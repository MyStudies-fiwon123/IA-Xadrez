using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightMovement : Movement
{
    public override List<Tile> GetValidMoves()
    {
        List<Tile> moves = new List<Tile>();

        moves.AddRange(GetMovement(new Vector2Int(0, 1)));
        moves.AddRange(GetMovement(new Vector2Int(0, -1)));
        moves.AddRange(GetMovement(new Vector2Int(1, 0)));
        moves.AddRange(GetMovement(new Vector2Int(-1, 0)));

        return moves;
    }
    List<Tile> GetMovement(Vector2Int direction)
    {
        List<Tile> moves = new List<Tile>();
        Tile current = Board.instance.selectedPiece.tile;
        Tile temp = GetTile(current.pos + direction * 2);
        if (temp != null){
            moves.AddRange(GetCurvedPart(temp.pos, new Vector2Int(direction.y, direction.x)));
        }
        return moves;
    }

    private IEnumerable<Tile> GetCurvedPart(Vector2Int pos, Vector2Int direction)
    {
        List<Tile> tiles = new List<Tile>();
        Tile tile1 = GetTile(pos+direction);
        Tile tile2 = GetTile(pos-direction);
        if (tile1 != null && (tile1.content == null) || IsEnemy(tile1))
            tiles.Add(tile1);
        if (tile2 != null && (tile1.content == null) || IsEnemy(tile2))
            tiles.Add(tile2);
        return tiles;
    }
}
