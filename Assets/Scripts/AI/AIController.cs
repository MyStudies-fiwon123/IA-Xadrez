using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class AIController : MonoBehaviour
{
    public static AIController instance;
    public Ply currentState;
    public HighlightClick AIhighlight;
    int calculationCount;
    public Ply minPly;
    public Ply maxPly;
    public int objectivePlyDepth = 2;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        instance = this;
    }

    [ContextMenu("Calculate Plays")]
    public async void CalculatePlays()
    {
        int minimaxDirection = 1;
        currentState = CreateSnapShot();
        currentState.name = "start";
        calculationCount = 0;

        Ply currentPly = currentState;
        currentPly.originPly = null;
        int currentPlyDepth = 0;
        currentPly.changes = new List<AffectedPiece>();

        Debug.Log("Começo");
        Task<Ply> calculation = CalculatePly(currentPly,
            GetTeam(currentPly, minimaxDirection),
            currentPlyDepth,
            minimaxDirection);
        await calculation;
        currentPly.bestFuture = calculation.Result;
        
        Debug.LogFormat("Melhor jogada para o dourado: {0}, com score: {1}",
            currentPly.bestFuture.name, currentPly.bestFuture.score);
        Debug.Log("Calculations: " + calculationCount);
    }
    async Task<Ply> CalculatePly(Ply parentPly, List<PieceEvaluation> team, int currentPlyDepth, int minimaxDirection)
    {
        parentPly.futurePlies = new List<Ply>();

        currentPlyDepth++;
        if (currentPlyDepth > objectivePlyDepth)
        {
            EvaluateBoard(parentPly);
            return parentPly;
        }
        Ply plyceHolder = new Ply();
        plyceHolder.score = -99999 * minimaxDirection;
        parentPly.bestFuture = plyceHolder;

        foreach (PieceEvaluation eva in team)
        {
            Debug.Log("analisando eva de: " + eva.piece);
            foreach (Tile t in eva.availableMoves)
            {
                calculationCount++;
                Debug.Log("Analisando t: " + t.pos);
                Board.instance.selectedPiece = eva.piece;
                Board.instance.selectedHighlight = AIhighlight;
                AIhighlight.tile = t;
                AIhighlight.transform.position = new Vector3(t.pos.x, t.pos.y, 0);
                TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
                PieceMovementState.MovePiece(tcs, true);
                await tcs.Task;
                Ply newPly = CreateSnapShot();
                newPly.name = string.Format("{0} , {1} to {2}",
                    parentPly.name, eva.piece.transform.parent.name + eva.piece.name, t.pos);
                newPly.changes = PieceMovementState.changes;
                // Debug.Log(newPly.name);
                Task<Ply> calculation = CalculatePly(newPly,
                    GetTeam(newPly, minimaxDirection * -1),
                    currentPlyDepth, minimaxDirection * -1);
                await calculation;
                parentPly.bestFuture = IsBest(parentPly.bestFuture, minimaxDirection, calculation.Result);

                newPly.originPly = parentPly;
                newPly.moveType = t.moveType;
                parentPly.futurePlies.Add(newPly);
                ResetBoardBackwards(newPly);
            }
        }
        return parentPly.bestFuture;
    }
    List<PieceEvaluation> GetTeam(Ply ply, int minimaxDirection)
    {
        if (minimaxDirection == 1)
            return ply.golds;
        else
            return ply.greens;
    }

    Ply IsBest(Ply ply, int minimaxDirection, Ply potentialBest)
    {
        if (minimaxDirection == 1)
        {
            if (potentialBest.score > ply.score)
                return potentialBest;
            return ply;
        }
        else
        {
            if (potentialBest.score < ply.score)
                return potentialBest;
            return ply;
        }
    }
    Ply CreateSnapShot()
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

        return ply;
    }

    PieceEvaluation CreateEvaluationPiece(Piece piece, Ply ply)
    {
        PieceEvaluation eva = new PieceEvaluation();
        eva.piece = piece;
        Board.instance.selectedPiece = eva.piece;
        eva.availableMoves = eva.piece.movement.GetValidMoves();
        return eva;
    }

    void EvaluateBoard(Ply ply)
    {

        foreach (PieceEvaluation piece in ply.golds)
        {
            EvaluatePiece(piece, ply, 1);
        }
        foreach (PieceEvaluation piece in ply.greens)
        {
            EvaluatePiece(piece, ply, 1);
        }
        Debug.Log("Board score: " + ply.score);
    }

    private void EvaluatePiece(PieceEvaluation eva, Ply ply, int scoreDirection)
    {

        eva.score = eva.piece.movement.value;
        ply.score += eva.score * scoreDirection;
    }

    void ResetBoardBackwards(Ply ply)
    {
        foreach (AffectedPiece p in ply.changes)
        {
            p.piece.tile.content = null;
            p.piece.tile = p.from;
            p.from.content = p.piece;
            p.piece.transform.position = new Vector3(p.from.pos.x, p.from.pos.y, 0);
            p.piece.gameObject.SetActive(true);
        }
    }
    [ContextMenu("Reset test")]
    void ResetBoard()
    {
        currentState.changes = PieceMovementState.changes;
        ResetBoard(currentState);
    }
}
