using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Movement : MonoBehaviour
{
    public abstract List<Tile> GetValidMoves();

    protected bool IsEnemy(Tile tile)
    {
        if (tile != null && tile.content != null && tile.content.transform.parent != Board.instance.selectedPiece.transform.parent)
        {
            return true;
        }

        return false;
    }
    protected Tile GetTile(Vector2Int position)
    {
        Tile tile;
        Board.instance.tiles.TryGetValue(position, out tile);
        return tile;
    }
    protected List<Tile> UntilBlockedPath(Vector2Int direction, bool includeBlocked, int limit)
    {
        List<Tile> moves = new List<Tile>();
        Tile current = Board.instance.selectedPiece.tile;
        while (current != null && moves.Count < limit){
            if (Board.instance.tiles.TryGetValue(current.pos + direction, out current)){
                if (current.content == null){
                    moves.Add(current);
                } else if (IsEnemy(current)){
                    if (includeBlocked)
                        moves.Add(current);
                    return moves;
                } else{ // Era um aliado
                    return moves;
                }
            }
        }
        return moves;
    }

    protected void SetNormalMove(List<Tile> tiles){
        foreach (Tile t in tiles){
            t.moveType = MoveType.Normal;
        }
    }
}
