using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ply
{
    public List<PieceEvaluation> golds;
    public List<PieceEvaluation> greens;
    public int score;
    public List<AffectedPiece> changes;
    public MoveType moveType;
    public Ply originPly;
    public List<Ply> futurePlies;
    public AvailableMove enPassantFlag;
    public Ply bestFuture;
}
