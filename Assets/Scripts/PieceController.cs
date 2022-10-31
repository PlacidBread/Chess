using System;
using UnityEngine;
public class PieceController : MonoBehaviour {
    private GameObject _selectedAss;

    public GameObject squareKillable;
    public GameObject squareMovable;
    public GameObject squareSelected;
    public GameObject darkSquare;
    public GameObject lightSquare;

    private GameObject _parentUI;
    
    public Camera cam;

    private bool _pieceSelected;

    private void Start() {
        _parentUI = GameObject.Find("UI");
        _pieceSelected = false;
    }
    
    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            HandleSelection();
        }
    }

    private void HandleSelection() {
        if (!_pieceSelected) {
            WaitForClick();
        }
    }

    private void WaitForClick() {
        // Vector2 mousePos = (Vector2)cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 rawMousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.transform.position.z * -1);
        Vector2 mousePos = (Vector2)cam.ScreenToWorldPoint(rawMousePos);
        // adjust pos for whatever reason (it works)
        mousePos.x += 0.5f;
        mousePos.y += 0.5f;
        if (!ValidPos(Vector2Int.RoundToInt(mousePos))) return;
        
        try {
            Collider2D targetObject = Physics2D.OverlapPoint(mousePos);
            if (targetObject) {
                // _selectedAss = targetObject.transform.gameObject;
                int raw = GameController.ToRawNum(new Vector2(Mathf.Floor(mousePos.x), Mathf.Floor(mousePos.y)));
                TileProps tileProps = GameController.arrayTile[raw];
                Debug.Log(tileProps.ToString());
                if (tileProps.pid != 0) {
                    Instantiate(squareSelected, new Vector2(tileProps.vec.x, tileProps.vec.y),
                        Quaternion.identity);
                    
                    Piece piece = DecidePiece(tileProps.pid);
                    piece.MakeMoves(); 
                    var moves = piece.GetMoves();
                    var kills = piece.GetKills();
                    
                    // if ((moves == null || moves.Length == 0) && (kills == null || kills.Length == 0)) {
                    //     return; }
                    
                    // piece.OutputMoves();
                    piece.MakeHMoves();
                    piece.OutputHMoves();

                    if (moves != null && moves.Length != 0) {
                        foreach (var move in moves) {
                            CreateUI(squareMovable, move);
                        }
                    }

                    if (kills == null || kills.Length == 0) return;
                    foreach (var kill in kills) {
                        CreateUI(squareKillable, kill.Item2);
                    }
                    // _pieceSelected = true;
                }
            }
        }
        catch (NullReferenceException e) {
            Debug.Log("NullReferenceException (no mouse attached?): " + e.Message);
            Debug.Log(e.StackTrace);
        }
        catch (Exception e) {
            Debug.Log(e.Message);
            Debug.Log(e.StackTrace);
        }
    }

    private Piece DecidePiece(int pid) {
        var pieceProps = GameController.pidToPiece[pid];
        
        // same as switch case
        Piece piece = pieceProps.type switch {
            PieceType.King => new King(pid),
            PieceType.Queen => new Queen(pid),
            PieceType.Rook => new Rook(pid),
            PieceType.Bishop => new Bishop(pid),
            PieceType.Knight => new Knight(pid),
            PieceType.Pawn => new Pawn(pid),
            PieceType.None => throw new Exception("PieceType not valid."),
            _ => throw new Exception("PieceType not valid."),
        };

        return piece;
    }

    private GameObject CreateUI(GameObject obj, Vector2 pos) {
        var newObj = Instantiate(obj, new Vector3(pos.x, pos.y, 0), Quaternion.identity);
        newObj.transform.parent = _parentUI.transform;

        return newObj;
    }
    public static bool ValidPos(Vector2Int pos) {
        if (pos.x is < 0 or > 7) return false;
        return pos.y is >= 0 and <= 7;
    }
    
    public static bool ValidPos(int x, int y) {
        if (x is < 0 or > 7) return false;
        return y is >= 0 and <= 7;
    }
}
