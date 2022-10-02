using System;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    
    public GameObject bKing;
    public GameObject bQueen;
    public GameObject bRook;
    public GameObject bBishop;
    public GameObject bKnight;
    public GameObject bPawn;

    public GameObject wKing;
    public GameObject wQueen;
    public GameObject wRook;
    public GameObject wBishop;
    public GameObject wKnight;
    public GameObject wPawn;
    
    public GameObject lightSquare;
    public GameObject darkSquare;

    private GameObject _parentBoard;
    private GameObject _parentPieces;
    
    private TileProps[] _tilesProps;

    private static readonly int[,] RotationMatrix = {
        { 0, 8, 16, 24, 32, 40, 48, 56 },
        { 1, 9, 17, 25, 33, 41, 49, 57 },
        { 2, 10, 18, 26, 34, 42, 50, 58 },
        { 3, 11, 19, 27, 35, 43, 51, 59 },
        { 4, 12, 20, 28, 36, 44, 52, 60 },
        { 5, 13, 21, 29, 37, 45, 53, 61 },
        { 6, 14, 22, 30, 38, 46, 54, 62 },
        { 7, 15, 23, 31, 39, 47, 55, 63 }
    };
    
    private void Start() {
        _parentBoard = GameObject.Find("Board");
        _parentPieces = GameObject.Find("Pieces");
        _tilesProps = DrawBoard();
        FenUtility fen = new FenUtility();
        fen.FenToBoard("default");
        //fen.OutputInfo();
        DrawPieces(fen);
        for (int i = 0; i < 64; i++) {
            Debug.Log(i + " : " + _tilesProps[i].id);
        }
        
    }
    
    // column = file (1, 2...), row = rank (a, b...)
    private TileProps[] DrawBoard() {
        TileProps[] tilesProps = new TileProps[64];
        for (int file = 0; file < 8; file++) {
            for (int rank = 0; rank < 8; rank++) {
                
                int rPos = RotationMatrix[file, rank];
                bool isLight = (file + rank) % 2 != 0;
                
                GameObject obj = isLight ? lightSquare : darkSquare;
                
                TileProps tileProps = (TileProps)obj.GetComponent("TileProps");
                tileProps.id = (rPos); // assign id
                tileProps.pos = intToPos(tileProps.id);
                tileProps.vec = posToVec(tileProps.pos);
                Vector2 position = tileProps.vec;
                
                tilesProps[rPos] = tileProps;
                
                Create(obj, position);
            }
        }

        return tilesProps;
    }

    private void DrawPieces(FenUtility fen) {
        for (int i = 0; i < 64; i++) {
            Vector2 pos = posToVec(intToPos(i));
            // Debug.Log(pos);
            bool isBlack = false;
            if (fen._posInfo.colours[i] > -1)
                isBlack = fen._posInfo.colours[i] == 1;
            // type casting int to enum
            switch ((Piece)fen._posInfo.tiles[i]) {
                // conduct fewer comparisons by checking none first 
                case Piece.None:
                    break;
                case Piece.King:
                    Create(isBlack ? bKing : wKing, pos);
                    break;
                case Piece.Queen:
                    Create(isBlack ? bQueen : wQueen, pos);
                    break;
                case Piece.Rook:
                    Create(isBlack ? bRook: wRook, pos);
                    break;
                case Piece.Bishop:
                    Create(isBlack ? bBishop : wBishop, pos);
                    break;
                case Piece.Knight:
                    Create(isBlack ? bKnight : wKnight, pos);
                    break;
                case Piece.Pawn:
                    Create(isBlack ? bPawn : wPawn, pos);
                    break;
            }
        }
    }
        
    // TODO: create dictionary for looking up gameObject based on index
    private void Create(GameObject obj, Vector2 pos) {
        // ObjectLookup
        var newObj = Instantiate(obj, new Vector3(pos.x, pos.y, 0), Quaternion.identity);
        // ensure that new objects are created under correct parent
        if (obj == lightSquare || obj == darkSquare) {
            newObj.transform.parent = _parentBoard.transform; 
        }
        else {
            newObj.transform.parent = _parentPieces.transform; 
        }
    }
    // ----------------------- //
    //    Utility functions    //
    
    // The two functions below will usually be called together, but are separate so the square number can be identified
    public int[] intToPos(int numPos) {
        int[] fileRank = new int[2];
        // case 63: 63/8 = 7 floored, 63 % 8 = r7
        // case 44: 44/8 = 5 floored, 44 % 8 = r4
        // case 1: 1/8 = 0 floored, 1 % 8 = r1
        // case 0: 0, r0
        int file = numPos % 8;
        int rank = (int)Math.Floor((numPos / 8.0d));
        
        fileRank[0] = 'a' + file;
        fileRank[1] = 8 - rank; // flip position so descends from top left -> bottom right
            
        // Debug.Log(fileRank[0].ToString());
        return fileRank;
    }

    public Vector2 posToVec(int[] posses) {
        // column = file, row = rank (different context so names different)
        int column = posses[0] - 'a'; // ASCII a == 97
        int row = posses[1] - 1;

        return new Vector2(column, row);
    }

    // public int toRawNum(int a, int b) {
    //     int eval = RotationMatrix[a, b];
    //     return eval;
    // }
}