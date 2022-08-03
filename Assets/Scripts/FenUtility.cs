using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chess {
    public class FenUtility {
        private Dictionary<char, int> mapPiece = new Dictionary<char, int>() {
            {'k', Piece.King}, {'q', Piece.Queen}, {'r', Piece.Rook}, {'b', Piece.Bishop}, 
            {'n', Piece.Knight}, {'p', Piece.Pawn}
        };
        private const string startFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        public PosInfo _posInfo = new PosInfo();
        
        public void FenToBoard(string fen) {
            if (fen.Equals("default")) { fen = startFen; }
            
            string[] fArray = fen.Split(" ");
            
            int rank = 7;
            int file = 0;
            
            
            
            foreach (var c in fArray[0]) {
                // Debug.Log(c + " " + ((rank + 1) * 8 - file - 1));
                if (c == '/') {
                    rank--;
                    file = 0;
                    continue;
                }
                int index = ((rank + 1) * 8) - file - 1;
                if (Char.IsDigit(c)) {
                    int tmp = c - '0';
                    for (int i = 0; i < tmp; i++) {
                        _posInfo.tiles[index] = Piece.None;
                        _posInfo.colours[index] = -1;
                        file++;
                    }
                }
                else {
                    char newc; // use as c is immutable
                    if (Char.IsUpper(c)) {
                        _posInfo.colours[index] = 0;
                       newc = Char.ToLower(c);
                    }
                    else {
                        _posInfo.colours[index] = 1;
                        newc = c;
                    }
                    _posInfo.tiles[index] = mapPiece[Char.ToLower(newc)];
                    file++;
                }
            }
        }
        public void OutputInfo() {
            Debug.Log("ran");
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
        public int[] colours; // -1 = none, 0 = white, 1 = black

        public PosInfo() {
            tiles = new int[64];
            colours = new int[64];
        }
    }
}
