
using System;
using Unity.VisualScripting;
using UnityEngine;

public class King : Piece {
    private static readonly Vector2Int[] Translations = { Vector2Int.up, new (1, 1), Vector2Int.right, new (1, -1), Vector2Int.down, new (-1, -1), Vector2Int.left, new (-1, 1), };
    public King(int pid) : base(pid) {}
    public override void MakeMoves() {
        foreach (var t in Translations) {
            GenMoves(t);
        }
    }
}

public class Queen : Piece {
    private static readonly Vector2Int[] Translations = { Vector2Int.up, new (1, 1), Vector2Int.right, new (1, -1), Vector2Int.down, new (-1, -1), Vector2Int.left, new (-1, 1), };

    public Queen(int pid) : base(pid) {}
    public override void MakeMoves() {
        foreach (var t in Translations) {
            GenMoves(t);
        }
    }
}
public class Rook : Piece {
    private static readonly Vector2Int[] Translations = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

    public Rook(int pid) : base(pid) {}
    public override void MakeMoves() {
        foreach (var t in Translations) {
            GenMoves(t);
        }
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

        Debug.Log(pieceProps.isWhite);
        // for (int i = 0; i < tmpTranslations.Length; i++) {
        //     if (!pieceProps.isWhite) {
        //         tmpTranslations[i].y *= -1;
        //     }
        // }

        foreach (var kill in pieceProps.isWhite ? KillTranslationsW : KillTranslationsB) {
            Vector2Int nextPos = new Vector2Int(pieceProps.tVec.x + kill.x, pieceProps.tVec.y + kill.y);
            if (!PieceController.ValidPos(nextPos)) return;
            int aPid = GameController.arrayTile[GameController.ToRawNum(nextPos)].pid;
            Debug.Log(GameController.ToRawNum(nextPos));
            if (aPid != -1) {
                CheckAndKill(aPid);
            }
        }
    }
}

