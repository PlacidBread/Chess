using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FenUtility {
    public static bool fenCreated = false;
    
    private static readonly Dictionary<char, PieceType> mapPiece = new () {
        {'k', PieceType.King}, {'q', PieceType.Queen}, {'r', PieceType.Rook}, {'b', PieceType.Bishop}, 
        {'n', PieceType.Knight}, {'p', PieceType.Pawn}
    };
    public const string StartFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    public static PosInfo _posInfo;

    public static Fen fenObj;

    // private static string _lastFen;
        
    public static bool FenToBoard(string fen) {
        _posInfo = new PosInfo();
        
        if (fen.Equals("default")) { fen = StartFen; }
        string[] fArray = fen.Split(" ");
        if (fArray.Length != 6) {
            return false; }

        fenObj = new Fen(fArray);
        if (!fenObj.success) {
            return false;
        }

        int rank = 0;
        int file = 0;
        int count = 0;
        
        // TODO: instead of going through this system to determine piece positions, do it in a more direct way from fenObj.positions?
        foreach (var c in fenObj.positions) {
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
        
        return count == 8;
        
        // TODO: implement handling of remaining sections in fen string
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

public readonly struct Fen {
    public readonly string positions;
    public readonly ActiveColour activeColour;
    public readonly List<char> castling;
    public readonly string enPassant; // give default 
    public readonly int halfClock;
    public readonly int fullClock;

    public readonly bool success;

    private void OutputFen() {
        Debug.Log("positions: " + positions + '\n' + "active colour: " + activeColour + '\n'
                  + "castling: " + castling + " en passant: " + enPassant + '\n'
                  + "half clock: " + halfClock + " full clock: " + fullClock);
    }

    public Fen(string[] fenArray) {
        var failCount = 0;
        castling = new List<char>();
        positions = fenArray[0];
        char[] _castleChars = { 'K', 'Q', 'k', 'q' };
        
        
        if (!char.TryParse(fenArray[1], out var tmpColour)) { failCount++; }
        switch (tmpColour) {
            case 'w':
                activeColour = ActiveColour.White;
                break;
            case 'b':
                activeColour = ActiveColour.Black;
                break;
            default:
                activeColour = ActiveColour.None;
                failCount++;
                break;
        }
        
        // castling = fenArray[2];
        
        if (string.IsNullOrWhiteSpace(fenArray[2])) {
            
        }
        else {
            foreach (var c in fenArray[2]) {
                if (_castleChars.Contains(c)) {
                    castling.Add(c);
                }
                else if (c == '-') {
                    castling.Clear();
                    castling.Add('-');
                }
            }
        }        
        enPassant = fenArray[3];
        if (string.IsNullOrWhiteSpace(enPassant)) {
            enPassant = "-"; }
        if (!int.TryParse(fenArray[4], out halfClock)) { failCount++; }
        if (!int.TryParse(fenArray[5], out fullClock)) { failCount++; }
        
        success = failCount == 0;
        OutputFen();
    }

    // public struct CastlingInfo {
    //     public bool wCastleKing;
    //     public bool wCastleQueen;
    //     public bool bCastleKing;
    //     public bool bCastleQueen;
    //
    //     CastlingInfo(string s) {
    //         if (string.IsNullOrWhiteSpace(s)) {
    //         
    //         }
    //         else {
    //             foreach (var c in s) {
    //                 if (_castleChars.Contains(c)) {
    //                     castling.Add(c);
    //                 }
    //                 else if (c == '-') {
    //                     castling.Clear();
    //                     castling.Add('-');
    //                 }
    //             }
    //         }
    //     }
    // }
}