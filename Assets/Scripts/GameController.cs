using System;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    
    [NonSerialized]
    public GameObject[] _tiles;
    [NonSerialized]
    public List<GameObject> pieces;
    
    private static int _id;

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
    
    // private static readonly int[,] VecMatrix = {
    //     { 0, 1, 2, 3, 4, 5, 6, 7 },
    //     { 8, 9, 10, 11, 12, 13, 14, 15 },
    //     { 16, 17, 18, 19, 20, 21, 22, 23},
    //     { 24, 25, 26, 27, 28, 29, 30, 31 },
    //     { 32, 33, 34, 35, 36, 37, 38, 39 },
    //     { 40, 41, 42, 43, 44, 45, 46, 47 },
    //     { 48, 49, 50, 51, 52, 53, 54, 55 },
    //     { 56, 57, 58, 59, 60, 61, 62, 63}
    // };

    private static readonly int[,] BetterMatrix = {
        { 56, 48, 40, 32, 24, 16, 8, 0 },
        { 57, 49, 41, 33, 25, 17, 9, 1 },
        { 58, 50, 42, 34, 26, 18, 10, 2 },
        { 59, 51, 43, 35, 27, 19, 11, 3 },
        { 60, 52, 44, 36, 28, 20, 12, 4 },
        { 61, 53, 45, 37, 29, 21, 13, 5 },
        { 62, 54, 46, 38, 30, 22, 14, 6 },
        { 63, 55, 47, 39, 31, 23, 15, 7 }
    };

    private void Start() {
        _parentBoard = GameObject.Find("Board");
        _parentPieces = GameObject.Find("Pieces");
        _tiles = new GameObject[64];
        pieces = new List<GameObject>();
        DrawBoard();
        FenUtility fen = new FenUtility();
        fen.FenToBoard("default");
        //fen.OutputInfo();
        
        for (int i = 0; i < 64; i++) {
            DrawPiece(fen, i);
        }
        // for (int i = 0; i < 64; i++) {
        //     TileProps tp =  _tiles[i].GetComponent<TileProps>();
        //     tp.OutputInfo();
        // }
        
    }
    
    // column = file (1, 2...), row = rank (a, b...)
    private void DrawBoard() {
        for (int file = 0; file < 8; file++) {
            for (int rank = 0; rank < 8; rank++) {
                
                int rPos = RotationMatrix[file, rank];
                bool isLight = (file + rank) % 2 != 0;
                
                GameObject obj = isLight ? lightSquare : darkSquare;
                
                TileProps tileProps = obj.GetComponent<TileProps>();
                tileProps.id = (rPos); // assign id
                tileProps.pos = intToPos(tileProps.id);
                tileProps.vec = posToVec(tileProps.pos);
                Vector2 position = tileProps.vec;

                Create(obj, position, tileProps);
            }
        }
    }

    private void DrawPiece(FenUtility fen, int i) {
        Vector2 pos = posToVec(intToPos(i));
        GameObject piece;
        Piece pieceType = (Piece)fen._posInfo.tiles[i];
        bool isWhite = fen._posInfo.colours[i];
        
        
        Debug.Log(fen._posInfo.colours[i]);
        // conduct fewer comparisons by checking none first 
        if (pieceType == Piece.None) {
            return;
        }

        switch (pieceType) {
            case Piece.King:
                piece = Create(isWhite ? wKing : bKing, pos);
                break;
            case Piece.Queen:
                piece = Create(isWhite ? wQueen : bQueen, pos);
                break;
            case Piece.Rook:
                piece = Create(isWhite ? wRook: bRook, pos);
                break;
            case Piece.Bishop:
                piece = Create(isWhite ? wBishop : bBishop, pos);
                break;
            case Piece.Knight:
                piece = Create(isWhite ? wKnight : bKnight, pos);
                break;
            case Piece.Pawn:
                piece = Create(isWhite ? wPawn : bPawn, pos);
                break;
            default:
                return;
        }

        TileProps tileProps = _tiles[ToRawNum(pos)].GetComponent<TileProps>();
        tileProps.pid = _id;
        
        var pieceProps = piece.GetComponent<PieceProps>();
        pieceProps.id = _id;
        pieceProps.type = pieceType;
        pieceProps.isWhite = isWhite;
        _id++;
    }
        
    // TODO: create dictionary for looking up gameObject based on index
    private GameObject Create(GameObject obj, Vector2 pos, TileProps tp = null) {
        var newObj = Instantiate(obj, new Vector3(pos.x, pos.y, 0), Quaternion.identity);
        // ensure that new objects are created under correct parent
        if (obj == lightSquare || obj == darkSquare) {
            if (tp == null) {
                throw new ArgumentException("TileProps object required for tile creation");
            }
            TileProps tileProps = newObj.GetComponent<TileProps>();
            tileProps.id = tp.id;
            tileProps.pos = tp.pos;
            tileProps.vec = tp.vec;

            _tiles[tileProps.id] = newObj;
            
            newObj.transform.parent = _parentBoard.transform;
        }
        else {
            pieces.Add(newObj);

            newObj.transform.parent = _parentPieces.transform; 
        }

        return newObj;
    }
    
    // ----------------------- //
    //    Utility functions    //
    
    // The two functions below will usually be called together, but are separate so the square number can be identified
    private int[] intToPos(int numPos) {
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

    private Vector2 posToVec(int[] posses) {
        // column = file, row = rank (different context so names different)
        int column = posses[0] - 'a'; // ASCII a == 97
        int row = posses[1] - 1;

        return new Vector2(column, row);
    }

    
    public int ToRawNum(Vector2 vec) {
        return BetterMatrix[Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y)];
    }
}