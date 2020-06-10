using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ply
{
    public List<PieceEvaluation> golds;
    public List<PieceEvaluation> greens;
    public float score;
    public string name;
    public List<AffectedPiece> changes;
    public MoveType moveType;
    public Ply originPly;
    public List<Ply> futurePlies;
    public Ply bestFuture;
}
