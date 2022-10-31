
using System;
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
    private static readonly Vector2Int[] KillTranslations = { new (-1, 1), new (1, 1) };

    private bool IsStartingPos() {
        if (pieceProps.isWhite) {
            return pieceProps.tVec.y == 1;
        }

        return pieceProps.tVec.y == 6;
    }
    // add functionality for promotion
    public override void MakeMoves() {
        if (IsStartingPos()) {
            foreach (var t in TranslationS) {
                GenMoves(t);
            }
        }
        else {
            GenMoves(Translation);
        }
        
        // TODO: Generate kills using KillTranslations
    }
}

