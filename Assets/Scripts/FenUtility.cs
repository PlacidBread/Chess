using System;
using System.Collections.Generic;
using UnityEngine;

public class FenUtility {
    public static bool fenCreated = false;
    
    private static readonly Dictionary<char, PieceType> mapPiece = new () {
        {'k', PieceType.King}, {'q', PieceType.Queen}, {'r', PieceType.Rook}, {'b', PieceType.Bishop}, 
        {'n', PieceType.Knight}, {'p', PieceType.Pawn}
    };
    private const string StartFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    public static PosInfo _posInfo;

    // private static string _lastFen;
        
    public static bool FenToBoard(string fen) {
        _posInfo = new PosInfo();
        
        if (fen.Equals("default")) { fen = StartFen; }
        string[] fArray = fen.Split(" ");
            
        int rank = 0;
        int file = 0;
        int count = 0;
        
        foreach (var c in fArray[0]) {
            // Debug.Log(c + " " + ((rank + 1) * 8 - file - 1));

            // if (!PieceController.ValidPos(rank, file)) return false;

            if (c == '/') {
                if (count != 8) {
                    return false; }

                count = 0;
                rank++;
                file = 0;
                continue;
            }
            int index = ((rank + 1) * 8) - file - 1;
            if (Char.IsDigit(c)) {
                int tmp = c - '0';
                for (int i = 0; i < tmp; i++) {
                    _posInfo.tiles[index] = (int)PieceType.None;
                    file++;
                }

                count += tmp;
            }
            else {
                char newc;
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
                count++;
            }
        }
        
        if (count != 8) {
            return false; }
        
        // TODO: implement handling of remaining sections in fen string
        return true;
        // _lastFen = fen;
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