using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

public abstract class Piece {
    protected List<Vector2Int> moves;
    protected List<Tuple<int, Vector2Int>> kills; // list of pid kills
    protected readonly PieceProps pieceProps;
    
    private Tuple<char, int>[] _hMoves;

    protected Piece(int pid) {
        pieceProps = GameController.pidToPiece[pid];
        if (pieceProps == null) throw new Exception("pieceProps uninitialized.");
        
        moves = new List<Vector2Int>();
        kills = new List<Tuple<int, Vector2Int>>();
    }
    
    [CanBeNull]
    public Vector2Int[] GetMoves() {
        var isNullOrEmpty = moves?.Any() != true;
        return isNullOrEmpty ? null : moves.ToArray();
    }
    
    public Tuple<int, Vector2Int>[] GetKills() {
        var isNullOrEmpty = kills?.Any() != true;
        return isNullOrEmpty ? null : kills.ToArray();
    }

    public void OutputMoves() {
        foreach (Vector2Int move in moves) {
            Debug.Log(move);
        }
    }
    
    public void OutputHMoves() {
        foreach (Tuple<char, int> hMove in _hMoves) {
            Debug.Log(hMove.Item1 + "" + hMove.Item2);
        }
    }

    public void MakeHMoves() {
        if (moves == null) {
            throw new Exception("'moves' var null (uninitialized?)");
        }
        _hMoves = new Tuple<char, int>[moves.Count];
        int count = 0;
        
        foreach (Vector2Int vec in moves) {
            _hMoves[count++] = GameController.intToPos(GameController.ToRawNum(vec));
        }
    }

    public abstract void MakeMoves();

    protected void GenMoves(Vector2Int translation) {
        bool specialPiece = pieceProps.type is PieceType.Pawn or PieceType.Knight or PieceType.King;

        if (!pieceProps.isWhite && pieceProps.type != PieceType.Knight) {
            translation.x *= -1;
            translation.y *= -1;
        }

        Vector2Int nextPos = new Vector2Int(pieceProps.tVec.x + translation.x, pieceProps.tVec.y + translation.y);
        if (!PieceController.ValidPos(nextPos)) return;
        int aPid = GameController.arrayTile[GameController.ToRawNum(nextPos)].pid;

        // if there is piece in new pos
        if (aPid != -1) {
            CheckAndKill(aPid);
            return;
        }
        
        // for specified PieceType('s), only allow this one move through
        if (specialPiece) {
            moves.Add(nextPos);
            return;
        }
        
        while (aPid == -1) {
            moves.Add(nextPos);
            nextPos = new Vector2Int(nextPos.x + translation.x, nextPos.y + translation.y);
            if (!PieceController.ValidPos(nextPos)) return;
            aPid = GameController.arrayTile[GameController.ToRawNum(nextPos)].pid;
            if (aPid != -1) {
                CheckAndKill(aPid);
            }
        }
    }

    private void CheckAndKill(int pid) {
        PieceProps aPieceProps = GameController.pidToPiece[pid];
        if (aPieceProps.isWhite != pieceProps.isWhite) {
            kills.Add(new Tuple<int, Vector2Int>(pid, aPieceProps.tVec));
        }
    }
}

