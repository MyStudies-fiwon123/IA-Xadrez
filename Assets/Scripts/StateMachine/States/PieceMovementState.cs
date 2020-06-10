using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PieceMovementState : State
{
    public static List<AffectedPiece> changes;
    public override async void Enter()
    {
        Debug.Log("PlaceMovementState:");
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

        MovePiece(tcs, false);


        await tcs.Task;
        machine.ChangeTo<TurnEndState>();
    }
    public static void MovePiece(TaskCompletionSource<bool> tcs, bool skipMovement)
    {
        changes = new List<AffectedPiece>();
        MoveType moveType = Board.instance.selectedHighlight.tile.moveType;
        ClearEnPassants();

        switch (moveType)
        {
            case MoveType.Normal:
                NormalMove(tcs, skipMovement);
                break;
            case MoveType.Casting:
                Castling(tcs, skipMovement);
                break;
            case MoveType.PawnDoubleMove:
                PawnDoubleMove(tcs, skipMovement);
                break;
            case MoveType.EnPassant:
                EnPassant(tcs, skipMovement);
                break;
            case MoveType.Promotion:
                Promotion(tcs, skipMovement);
                break;
        }
    }

    static void ClearEnPassants()
    {
        ClearEnPassants(5);
        ClearEnPassants(2);
    }
    static void ClearEnPassants(int height)
    {
        Vector2Int positions = new Vector2Int(0, height);
        for (int i = 0; i < 7; i++)
        {
            positions.x = positions.x + 1;
            Board.instance.tiles[positions].moveType = MoveType.Normal;
        }
    }
    static void PawnDoubleMove(TaskCompletionSource<bool> tcs, bool skipMovement)
    {
        Piece pawn = Board.instance.selectedPiece;
        Vector2Int direction = pawn.tile.pos.y > Board.instance.selectedHighlight.tile.pos.y ?
            new Vector2Int(0, -1) :
            new Vector2Int(0, 1);
        Board.instance.tiles[pawn.tile.pos + direction].moveType = MoveType.EnPassant;
        NormalMove(tcs, skipMovement);
    }
    static void EnPassant(TaskCompletionSource<bool> tcs, bool skipMovement)
    {
        Piece pawn = Board.instance.selectedPiece;
        Vector2Int direction = pawn.tile.pos.y > Board.instance.selectedHighlight.tile.pos.y ?
            new Vector2Int(0, 1) :
            new Vector2Int(0, -1);
        Tile enemy = Board.instance.tiles[Board.instance.selectedHighlight.tile.pos + direction];
        enemy.content.gameObject.SetActive(false);
        enemy.content = null;
        NormalMove(tcs, skipMovement);
    }

    static void Castling(TaskCompletionSource<bool> tcs, bool skipMovement)
    {
        Piece king = Board.instance.selectedPiece;
        king.tile.content = null;
        Piece rook = Board.instance.selectedHighlight.tile.content;
        rook.tile.content = null;

        Vector2Int direction = rook.tile.pos - king.tile.pos;
        if (direction.x > 0)
        {
            king.tile = Board.instance.tiles[new Vector2Int(king.tile.pos.x + 2, king.tile.pos.y)];
            rook.tile = Board.instance.tiles[new Vector2Int(king.tile.pos.x - 1, king.tile.pos.y)];
        }
        else
        {
            king.tile = Board.instance.tiles[new Vector2Int(king.tile.pos.x - 2, king.tile.pos.y)];
            rook.tile = Board.instance.tiles[new Vector2Int(king.tile.pos.x + 1, king.tile.pos.y)];
        }
        king.tile.content = king;
        rook.tile.content = rook;
        king.wasMoved = true;
        rook.wasMoved = true;

        LeanTween.move(king.gameObject, new Vector3(king.tile.pos.x, king.tile.pos.y, 0), 1.5f).
            setOnComplete(() =>
            {
                tcs.SetResult(true);
            });

        LeanTween.move(rook.gameObject, new Vector3(rook.tile.pos.x, rook.tile.pos.y, 0), 1.4f);
    }

    static void NormalMove(TaskCompletionSource<bool> tcs, bool skipMovement)
    {
        Piece piece = Board.instance.selectedPiece;
        AffectedPiece pieceMoving = new AffectedPiece();
        pieceMoving.piece = piece;
        pieceMoving.from = piece.tile;
        changes.Add(pieceMoving);

        piece.tile.content = null;
        piece.tile = Board.instance.selectedHighlight.tile;

        if (piece.tile.content != null)
        {
            Piece deadPiece = piece.tile.content;
            AffectedPiece pieceKilled = new AffectedPiece();
            pieceKilled.piece = deadPiece;
            pieceKilled.from = piece.tile;
            changes.Add(pieceKilled);
            // Debug.LogFormat("Peça {0} foi morta", deadPiece.transform);
            deadPiece.gameObject.SetActive(false);
        }
        piece.tile.content = piece;

        if (skipMovement)
        {
            piece.transform.position = Board.instance.selectedHighlight.transform.position;
            tcs.SetResult(true);
        }
        else
        {
            piece.wasMoved = true;
            float timing = Vector3.Distance(piece.transform.position, Board.instance.selectedHighlight.transform.position) * 0.5f;
            LeanTween.move(piece.gameObject, Board.instance.selectedHighlight.transform.position, timing).
                setOnComplete(() =>
                {
                    tcs.SetResult(true);
                });
        }

    }
    static async void Promotion(TaskCompletionSource<bool> tcs, bool skipMovement)
    {
        TaskCompletionSource<bool> movementTCS = new TaskCompletionSource<bool>();
        NormalMove(movementTCS, skipMovement);
        await movementTCS.Task;
        Debug.Log("promotion");
        StateMachineController.instance.taskHold = new TaskCompletionSource<object>();
        StateMachineController.instance.promotionPanel.SetActive(true);

        await StateMachineController.instance.taskHold.Task;

        string result = StateMachineController.instance.taskHold.Task.Result as string;
        if (result == "Knight")
        {
            Board.instance.selectedPiece.movement = new KnightMovement();
        }
        else
        {
            Board.instance.selectedPiece.movement = new QueenMovement();
        }

        StateMachineController.instance.promotionPanel.SetActive(false);
        tcs.SetResult(true);
    }
}