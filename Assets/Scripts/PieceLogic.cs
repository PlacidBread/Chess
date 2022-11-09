
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class King : Piece {
    private static readonly Vector2Int[] Translations = { Vector2Int.up, new (1, 1), Vector2Int.right, 
        new (1, -1), Vector2Int.down, new (-1, -1), Vector2Int.left, new (-1, 1), };
    private List<Tuple<int, Vector2Int>> _castleMoves;

    public static bool kingMovedW = false;
    public static bool kingMovedB = false;

    private bool _canCastleQ = false;
    private bool _canCastleK = false;

    public King(int pid) : base(pid) {
        _castleMoves = new List<Tuple<int, Vector2Int>>();
    }

    public Tuple<int, Vector2Int>[] GetCastleMoves() {
        var isNullOrEmpty = _castleMoves?.Any() != true;
        return isNullOrEmpty ? null : _castleMoves.ToArray();
    }
    public override void MakeMoves() {
        foreach (var t in Translations) {
            GenMoves(t);
        }
        
        // if clear path to rook do below, else return
        
        if (FenUtility.fenObj.castling[0].Equals('-')) {
            return; }
        
        foreach (var c in FenUtility.fenObj.castling) {
            if (Char.IsUpper(c) && pieceProps.isWhite) {
                if (c.Equals('K')) _canCastleK = true;
                else {
                    _canCastleQ = true; 
                }
            }
            else if (Char.IsLower(c) && !pieceProps.isWhite) {
                if (c.Equals('k')) _canCastleK = true;
                else {
                    _canCastleQ = true; 
                }
            }
        }

        if (_canCastleQ) {
            AddCastleMoves(Vector2Int.left, false);
        }

        if (_canCastleK) {
            AddCastleMoves(Vector2Int.right, true);
        }
    }

    public static bool AllMoved() {
        return kingMovedB && kingMovedW;
    }

    private void AddCastleMoves(Vector2Int translation, bool kingSide) {
        Vector2Int nextPos = new Vector2Int(pieceProps.tVec.x + translation.x, pieceProps.tVec.y + translation.y);

        if (!PieceController.ValidPos(nextPos)) return;
        var aPid = GameController.arrayTile[GameController.ToRawNum(nextPos)].pid;
        
        while (aPid == -1) {
            nextPos = new Vector2Int(nextPos.x + translation.x, nextPos.y + translation.y);
            
            if (!PieceController.ValidPos(nextPos)) return; // check needed?
            aPid = GameController.arrayTile[GameController.ToRawNum(nextPos)].pid;
            if (aPid != -1) {
                var aPieceProps = GameController.pidToPiece[aPid];
                if (aPieceProps.type == PieceType.Rook) {
                    _castleMoves.Add(new Tuple<int, Vector2Int>(aPid, kingSide ? new Vector2Int(pieceProps.tVec.x + 2, pieceProps.tVec.y) : new Vector2Int(pieceProps.tVec.x - 2, pieceProps.tVec.y)));
                    return;
                }
            }
        }
    }
}

public class Queen : Piece {
    private static readonly Vector2Int[] Translations = { Vector2Int.up, new (1, 1), Vector2Int.right, 
        new (1, -1), Vector2Int.down, new (-1, -1), Vector2Int.left, new (-1, 1), };

    public Queen(int pid) : base(pid) {}
    public override void MakeMoves() {
        foreach (var t in Translations) {
            GenMoves(t);
        }
    }
}
public class Rook : Piece {
    private static readonly Vector2Int[] Translations = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

    public static bool rookMovedK = false;
    public static bool rookMovedQ = false; 
    public static bool rookMovedk = false; 
    public static bool rookMovedq = false;

    public Rook(int pid) : base(pid) {}
    public override void MakeMoves() {
        foreach (var t in Translations) {
            GenMoves(t);
        }
    }
    
    public static bool AllMoved() {
        return rookMovedk && rookMovedq && rookMovedK && rookMovedQ;
    }
}

public class Bishop : Piece {
    private static readonly Vector2Int[] Translations = { new (1, 1), new (1, -1), new (-1, -1), new (-1, 1)};

    public Bishop(int pid) : base(pid) {}
    public override void MakeMoves() {
        foreach (var t in Translations) {
            GenMoves(t);
        }
    }
}

public class Knight : Piece {
    private static readonly Vector2Int[] Translations = { new (-1, 2), new (1, 2), new (2, 1), new (2, -1), new (1, -2), new (-1, -2), new (-2, -1), new (-2, 1)};
    public Knight(int pid) : base(pid) {}
    public override void MakeMoves() {
        foreach (var t in Translations) {
            GenMoves(t);
        }
    }
}

public class Pawn : Piece {
    // calls base automatically
    public Pawn(int pid) : base(pid) {}
    
    private static readonly Vector2Int Translation = Vector2Int.up;
    private static readonly Vector2Int[] TranslationS = { Vector2Int.up, new(0, 2) };
    private static readonly Vector2Int[] KillTranslationsW = { new (-1, 1), new (1, 1) };
    private static readonly Vector2Int[] KillTranslationsB = { new (-1, -1), new (1, -1) };

    private bool IsStartingPos() {
        if (pieceProps.isWhite) {
            return pieceProps.tVec.y == 1;
        }

        return pieceProps.tVec.y == 6;
    }
    
    // add functionality for promotion
    public override void MakeMoves() {
        if (IsStartingPos()) {
            foreach (var move in TranslationS) {
                GenMoves(move, false);
            }
        }
        else {
            GenMoves(Translation, false);
        }
        
        foreach (var kill in pieceProps.isWhite ? KillTranslationsW : KillTranslationsB) {
            Vector2Int nextPos = new Vector2Int(pieceProps.tVec.x + kill.x, pieceProps.tVec.y + kill.y);
            if (!PieceController.ValidPos(nextPos)) continue;
            
            int aPid = GameController.arrayTile[GameController.ToRawNum(nextPos)].pid;
            if (aPid != -1) {
                CheckAndKill(aPid);
            }
        }
    }
}

