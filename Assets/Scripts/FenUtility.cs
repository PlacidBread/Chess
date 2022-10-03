using System;
using System.Collections.Generic;
using UnityEngine;

public class FenUtility {
    private Dictionary<char, Piece> mapPiece = new Dictionary<char, Piece>() {
        {'k', Piece.King}, {'q', Piece.Queen}, {'r', Piece.Rook}, {'b', Piece.Bishop}, 
        {'n', Piece.Knight}, {'p', Piece.Pawn}
    };
    private const string StartFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    public PosInfo _posInfo = new PosInfo();

    private static string _lastFen;
        
    public void FenToBoard(string fen) {
        if (fen.Equals("default")) { fen = StartFen; }
        string[] fArray = fen.Split(" ");
            
        int rank = 0;
        int file = 0;
        
        foreach (var c in fArray[0]) {
            // Debug.Log(c + " " + ((rank + 1) * 8 - file - 1));
            if (c == '/') {
                rank++;
                file = 0;
                continue;
            }
            int index = ((rank + 1) * 8) - file - 1;
            if (Char.IsDigit(c)) {
                int tmp = c - '0';
                for (int i = 0; i < tmp; i++) {
                    _posInfo.tiles[index] = (int)Piece.None;
                    file++;
                }
            }
            else {
                char newc; // use as c is immutable
                if (Char.IsUpper(c)) {
                    _posInfo.colours[index] = true;
                    newc = Char.ToLower(c);
                }
                else {
                    _posInfo.colours[index] = false;
                    newc = c;
                }
                _posInfo.tiles[index] = (int)mapPiece[Char.ToLower(newc)];
                file++;
            }
        }

        _lastFen = fen;
    }
    public void OutputInfo() {
        for (int i = 64; i > 0; i--) {
            if (i % 8 == 0) {
                Debug.Log("");
            }
            Debug.Log(_posInfo.tiles[i - 1].ToString());
        }
    }
}

public class PosInfo {
    public int[] tiles;
    public bool[] colours; // true (1) = white, false (0) = black

    public PosInfo() {
        tiles = new int[64];
        colours = new bool[64];
    }
}