using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PieceMovementState : State
{
    public static List<AffectedPiece> changes;
    public static AvailableMove enPassantFlag;
    public override async void Enter()
    {
        Debug.Log("PlaceMovementState:");
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

        MovePiece(tcs, false, Board.instance.selectedMove.moveType);


        await tcs.Task;
        machine.ChangeTo<TurnEndState>();
    }
    public static void MovePiece(TaskCompletionSource<bool> tcs, bool skipMovement, MoveType moveType)
    {
        changes = new List<AffectedPiece>();
        enPassantFlag = new AvailableMove();

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

    static void PawnDoubleMove(TaskCompletionSource<bool> tcs, bool skipMovement)
    {
        Piece pawn = Board.instance.selectedPiece;
        Vector2Int direction = pawn.maxTeam ?
            new Vector2Int(0, 1) :
            new Vector2Int(0, -1);

        enPassantFlag = new AvailableMove(pawn.tile.pos + direction, MoveType.EnPassant);
        NormalMove(tcs, skipMovement);
    }
    static void EnPassant(TaskCompletionSource<bool> tcs, bool skipMovement)
    {
        Piece pawn = Board.instance.selectedPiece;
        Vector2Int direction = pawn.maxTeam ?
            new Vector2Int(0, -1) :
            new Vector2Int(0, 1);


        Tile enemy = Board.instance.tiles[Board.instance.selectedMove.pos + direction];
        AffectedPiece affectedEnemy = new AffectedPiece();
        affectedEnemy.from = affectedEnemy.to = enemy;
        affectedEnemy.piece = enemy.content;
        changes.Add(affectedEnemy);
        enemy.content.gameObject.SetActive(false);
        enemy.content = null;

        NormalMove(tcs, skipMovement);
    }

    static void Castling(TaskCompletionSource<bool> tcs, bool skipMovement)
    {
        Piece king = Board.instance.selectedPiece;
        AffectedKingRook affectedKing = new AffectedKingRook();
        affectedKing.from = king.tile;
        king.tile.content = null;
        affectedKing.piece = king;

        Piece rook = Board.instance.tiles[Board.instance.selectedMove.pos].content;
        rook.tile.content = null;
        AffectedKingRook affectedRook = new AffectedKingRook();
        affectedRook.from = rook.tile;
        affectedKing.piece = rook;

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
        affectedKing.to = king.tile;
        changes.Add(affectedKing);
        rook.tile.content = rook;
        affectedRook.to = rook.tile;
        changes.Add(affectedRook);

        king.wasMoved = true;
        rook.wasMoved = true;

        if (skipMovement)
        {
            tcs.SetResult(true);
        }
        else
        {
            LeanTween.move(king.gameObject, new Vector3(king.tile.pos.x, king.tile.pos.y, 0), 1.5f).
                setOnComplete(() =>
                {
                    tcs.SetResult(true);
                });

            LeanTween.move(rook.gameObject, new Vector3(rook.tile.pos.x, rook.tile.pos.y, 0), 1.4f);
        }

    }

    static void NormalMove(TaskCompletionSource<bool> tcs, bool skipMovement)
    {
        Piece piece = Board.instance.selectedPiece;
        AffectedPiece pieceMoving = piece.CreateAffected();
        pieceMoving.piece = piece;
        pieceMoving.from = piece.tile;
        pieceMoving.to = Board.instance.tiles[Board.instance.selectedMove.pos];
        changes.Insert(0, pieceMoving);

        piece.tile.content = null;
        piece.tile = pieceMoving.to;

        if (piece.tile.content != null)
        {
            Piece deadPiece = piece.tile.content;
            AffectedPiece pieceKilled = new AffectedPiece();
            pieceKilled.piece = deadPiece;
            pieceKilled.from = pieceKilled.to = piece.tile;
            changes.Add(pieceKilled);
            // Debug.LogFormat("Peça {0} foi morta", deadPiece.transform);
            deadPiece.gameObject.SetActive(false);
        }
        piece.tile.content = piece;
        piece.wasMoved = true;

        Vector3 v3Pos = new Vector3(Board.instance.selectedMove.pos.x, Board.instance.selectedMove.pos.y, 0);
        if (skipMovement)
        {
            piece.wasMoved = true;
            //piece.transform.position = v3Pos;
            tcs.SetResult(true);
        }
        else
        {
            float timing = Vector3.Distance
                (piece.transform.position, v3Pos) * 0.5f;

            LeanTween.move(piece.gameObject, v3Pos, timing).
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
        // Debug.Log("promotion");
        Pawn pawn = Board.instance.selectedPiece as Pawn;

        if (!skipMovement)
        {
            StateMachineController.instance.taskHold = new TaskCompletionSource<object>();
            StateMachineController.instance.promotionPanel.SetActive(true);

            await StateMachineController.instance.taskHold.Task;

            string result = StateMachineController.instance.taskHold.Task.Result as string;
            if (result == "Knight")
            {
                Board.instance.selectedPiece.movement = pawn.knightMovement;
            }
            else
            {
                Board.instance.selectedPiece.movement = pawn.queenMovement;
            }
            StateMachineController.instance.promotionPanel.SetActive(false);
        }else{
            AffectedPawn affectedPawn = new AffectedPawn();
            affectedPawn.wasMoved = true;
            affectedPawn.resetMovement = true;
            affectedPawn.from = changes[0].from;
            affectedPawn.to = changes[0].to;
            affectedPawn.piece = pawn;

            changes[0] = affectedPawn;
            pawn.movement = pawn.queenMovement;
        }
        StateMachineController.instance.promotionPanel.SetActive(false);
        tcs.SetResult(true);
    }
}