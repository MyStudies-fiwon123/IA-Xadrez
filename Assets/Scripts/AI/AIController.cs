using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public static AIController instance;
    public Ply currentState;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        instance = this;
    }
    [ContextMenu("Create Evaluations")]
    public void CreateEvaluations()
    {
        Ply ply = new Ply();
        ply.golds = new List<PieceEvaluation>();
        ply.greens = new List<PieceEvaluation>();

        foreach (Piece p in Board.instance.goldPieces)
        {
            if (p.gameObject.activeSelf)
                ply.golds.Add(CreateEvaluationPiece(p, ply));
        }

        foreach (Piece p in Board.instance.greenPieces)
        {
            if (p.gameObject.activeSelf)
                ply.greens.Add(CreateEvaluationPiece(p, ply));
        }

        currentState = ply;
    }

    PieceEvaluation CreateEvaluationPiece(Piece piece, Ply ply)
    {
        PieceEvaluation eva = new PieceEvaluation();
        eva.piece = piece;
        return eva;
    }
    [ContextMenu("Evaluate")]
    public void EvaluateBoard(){
        Ply ply = currentState;

        foreach(PieceEvaluation piece in ply.golds){
            EvaluatePiece(piece, ply, 1);
        }
        foreach(PieceEvaluation piece in ply.greens){
            EvaluatePiece(piece, ply, 1);
        }
        Debug.Log(ply.score);
    }

    private void EvaluatePiece(PieceEvaluation eva, Ply ply, int scoreDirection)
    {
        Board.instance.selectedPiece = eva.piece;
        List<Tile> tiles = eva.piece.movement.GetValidMoves();
        eva.availableMoves = tiles.Count;

        eva.score = eva.piece.movement.value;
        ply.score += eva.score*scoreDirection;
    }
}
