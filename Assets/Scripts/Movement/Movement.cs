using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Movement : MonoBehaviour
{
    public int value;
    public abstract List<AvailableMove> GetValidMoves();

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
    protected List<AvailableMove> UntilBlockedPath(Vector2Int direction, bool includeBlocked, int limit)
    {
        List<AvailableMove> moves = new List<AvailableMove>();
        Tile current = Board.instance.selectedPiece.tile;
        while (current != null && moves.Count < limit){
            if (Board.instance.tiles.TryGetValue(current.pos + direction, out current)){
                if (current.content == null){
                    moves.Add(new AvailableMove(current.pos));
                } else if (IsEnemy(current)){
                    if (includeBlocked)
                        moves.Add(new AvailableMove(current.pos));
                    return moves;
                } else{ // Era um aliado
                    return moves;
                }
            }
        }
        return moves;
    }
}
