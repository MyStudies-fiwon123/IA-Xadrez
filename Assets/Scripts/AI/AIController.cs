using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class AIController : MonoBehaviour
{
    public static AIController instance;
    public HighlightClick AIhighlight;
    int calculationCount;
    public Ply minPly;
    public Ply maxPly;
    public int objectivePlyDepth = 2;
    float lastInterval;
    public AvailableMove enPassantSaved;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        instance = this;
        maxPly = new Ply();
        maxPly.score = 999999;
        minPly = new Ply();
        minPly.score = -999999;
    }

    [ContextMenu("Calculate Plays")]
    public async Task<Ply> CalculatePlays()
    {
        lastInterval = Time.realtimeSinceStartup;
        int minimaxDirection;
        if (StateMachineController.instance.currentlyPlaying == StateMachineController.instance.player1)
            minimaxDirection = 1;
        else
            minimaxDirection = -1;

        enPassantSaved = PieceMovementState.enPassantFlag;
        Ply currentPly = CreateSnapShot();
        calculationCount = 0;

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
        
        Debug.Log("Calculations: " + calculationCount);
        Debug.Log("Time: "+(Time.realtimeSinceStartup-lastInterval));
        PrintBestPly(currentPly.bestFuture);
        PieceMovementState.enPassantFlag = enPassantSaved;
        return currentPly.bestFuture;
    }
    async Task<Ply> CalculatePly(Ply parentPly, List<PieceEvaluation> team, int currentPlyDepth, int minimaxDirection)
    {
        parentPly.futurePlies = new List<Ply>();

        currentPlyDepth++;
        if (currentPlyDepth > objectivePlyDepth)
        {
            EvaluateBoard(parentPly);
            // Task evaluationTask = Task.Run(()=> EvaluateBoard(parentPly));
            // await evaluationTask;
            return parentPly;
        }
        if (minimaxDirection == 1){
            parentPly.bestFuture = minPly;
        } else {
            parentPly.bestFuture = maxPly;
        }
        
        Ply plyceHolder = new Ply();
        plyceHolder.score = -9999999 * minimaxDirection;
        parentPly.bestFuture = plyceHolder;

        foreach (PieceEvaluation eva in team)
        {
            Debug.Log("analisando eva de: " + eva.piece);
            foreach (AvailableMove move in eva.availableMoves)
            {
                calculationCount++;
                Board.instance.selectedPiece = eva.piece;
                Board.instance.selectedMove = move;
                TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
                PieceMovementState.MovePiece(tcs, true, move.moveType);

                await tcs.Task;
                Ply newPly = CreateSnapShot(parentPly);
                newPly.changes = PieceMovementState.changes;
                newPly.enPassantFlag = PieceMovementState.enPassantFlag;

                Task<Ply> calculation = CalculatePly(newPly,
                    GetTeam(newPly, minimaxDirection * -1),
                    currentPlyDepth, minimaxDirection * -1);
                await calculation;

                parentPly.bestFuture = IsBest(parentPly.bestFuture, minimaxDirection, calculation.Result);
                newPly.originPly = parentPly;
                parentPly.futurePlies.Add(newPly);

                PieceMovementState.enPassantFlag = parentPly.enPassantFlag;
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

    Ply CreateSnapShot(Ply parentPly)
    {
        Ply ply = new Ply();
        ply.golds = new List<PieceEvaluation>();
        ply.greens = new List<PieceEvaluation>();

        foreach (PieceEvaluation p in parentPly.golds)
        {
            if (p.piece.gameObject.activeSelf)
                ply.golds.Add(CreateEvaluationPiece(p.piece, ply));
        }

        foreach (PieceEvaluation p in parentPly.greens)
        {
            if (p.piece.gameObject.activeSelf)
                ply.greens.Add(CreateEvaluationPiece(p.piece, ply));
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
        ply.score += eva.piece.movement.value * scoreDirection;
    }

    void ResetBoardBackwards(Ply ply)
    {
        foreach (AffectedPiece p in ply.changes)
        {
            p.Undo();
        }
    }

    void PrintBestPly(Ply finalPly){
        Ply currentPly = finalPly;
        Debug.Log("Melhor Jogada:");
        while(currentPly.originPly != null){
            Debug.LogFormat("{0}-{1}->{2}",
                currentPly.changes[0].piece.transform.parent.name,
                currentPly.changes[0].piece.name,
                currentPly.changes[0].to.pos);

            currentPly = currentPly.originPly;
        }
    }
}
