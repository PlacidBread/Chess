using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Chess {
    public class GameController : MonoBehaviour {
        // private Dictionary<int, GameObject> mapIntSprite = new Dictionary<int, GameObject>() {
        //     {Piece.King, king}, {Piece.Queen, queen}, {Piece.Rook, rook}, {Piece.Bishop, bishop}, 
        //     {Piece.Knight, knight}, {Piece.Pawn, pawn}
        // };

        public GameObject bKing;
        public GameObject bQueen;
        public GameObject bRook;
        public GameObject bBishop;
        public GameObject bKnight;
        public GameObject bPawn;

        public GameObject lightSquare;
        public GameObject darkSquare;

        private GameObject _parentBoard;
        private GameObject _parentPieces;

        // private class ObjectLookup : MonoBehaviour {
        //     // black pieces
        //     public GameObject bKing;
        //     public GameObject bQueen;
        //     public GameObject bRook;
        //     public GameObject bBishop;
        //     public GameObject bKnight;
        //     public GameObject bPawn;
        //
        //     public GameObject wKing;
        //     public GameObject wQueen;
        //     public GameObject wRook;
        //     public GameObject wBishop;
        //     public GameObject wKnight;
        //     public GameObject wPawn;
        //     
        //     public GameObject lightSquare;
        //     public GameObject darkSquare;
        // }

        private enum ObjectLookup {
            BKing = 1,
            BQueen = 2,
            BRook = 3,
            BBishop = 4,
            BKnight = 5,
            BPawn = 6,
            WKing = 11,
            WQueen = 12,
            WRook = 13,
            WBishop = 14,
            WKnight = 15,
            WPawn = 16,
            
            LightSquare = 100,
            DarkSquare = 101
        }
        
        private void Start() {
            _parentBoard = GameObject.Find("Board");
            _parentPieces = GameObject.Find("Pieces");
            DrawBoard();
            FenUtility fen = new FenUtility();
            fen.FenToBoard("default");
            // fen.OutputInfo();
            DrawPieces(fen);
            
        }
    
        // column = file (1, 2...), row = rank (a, b...)
        private void DrawBoard() {
            for (int file = 0; file < 8; file++) {
                for (int rank = 0; rank < 8; rank++) {
                    bool isLight = (file + rank) % 2 != 0;
                
                    Vector2 position = new Vector2(file, rank);
                    GameObject obj = isLight ? lightSquare : darkSquare;
                    Create(obj, position);
                }
            }
        }

        private void DrawPieces(FenUtility fen) {
            for (int i = 0; i < 64; i++) {
                Vector2 pos = posToVec(intToPos(i));
                // Debug.Log(pos);         
                switch (fen._posInfo.tiles[i]) {
                    
                    case Piece.King:
                        Create(bKing, pos);
                        break;
                    case Piece.Queen:
                        Create(bQueen, pos);
                        break;
                    case Piece.Rook:
                        Create(bRook, pos);
                        break;
                    case Piece.Bishop:
                        Create(bBishop, pos);
                        break;
                    case Piece.Knight:
                        Create(bKnight, pos);
                        break;
                    case Piece.Pawn:
                        Create(bPawn, pos);
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
        
        // TODO: convert int straight to Vector2?
        public int[] intToPos(int numPos) {
            int[] rankFile = new int[2];
            // case 64: 63/8 = 7 floored, 63 % 8 = r7
            // case 1: 1/8 = 0 floored, 1 % 8 = r1
            // case 0: 0, r0
            int file = numPos % 8;
            int rank = (int)Math.Floor((numPos / 8.0d));
            // rank++;
            // file++;

            rankFile[0] = 'a' + file;
            rankFile[1] = 8 - rank; // flip position so descends from top left -> bottom right
            
            Debug.Log(rankFile[1].ToString());
            return rankFile;
        }

        public Vector2 posToVec(int[] posses) {
            // column = file, row = rank (different context so names different)
            int column = posses[0] - 'a'; // ASCII a == 97
            int row = posses[1] - 1;

            return new Vector2(column, row);
        }
    }
}

