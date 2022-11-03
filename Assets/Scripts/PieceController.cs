using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
public class PieceController : MonoBehaviour {
    private GameObject _selectedAss;

    public GameObject squareKillable;
    public GameObject squareMovable;
    public GameObject squareSelected;
    // public GameObject darkSquare;
    // public GameObject lightSquare;

    private GameObject _parentUI;
    private List<GameObject> _ui;

    public Camera cam;

    [NonSerialized] public static MovementState movementState;
    [NonSerialized] public static ActiveColour activeColour;

    private Piece _piece;
    private int _pidRef;
    private int _tidRef;

    private void Start() {
        _parentUI = GameObject.Find("UI");
        _ui = new List<GameObject>();
        movementState = MovementState.None;
        activeColour = ActiveColour.None; // TODO: use to implement turns
        _pidRef = -1;
        _tidRef = -1;
    }
    
    private void Update() {
        if (!FenUtility.fenCreated) return;
        
        try {
            if (Input.GetMouseButtonDown(0)) {
                Debug.Log(movementState);
                // TODO: ensure coroutine is only being called once every input
                StartCoroutine(WaitForUser());
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


    private bool OnClick() {
        Vector2Int mousePos;
        TileProps tileProps;
        switch (movementState) {
            case MovementState.WaitSelect:
                mousePos = Vector2Int.RoundToInt(GetMouseInfo());
                if (!ValidPos(mousePos)) {
                    return false;
                }

                var tid = GameController.ToRawNum(new Vector2(Mathf.Floor(mousePos.x), Mathf.Floor(mousePos.y)));
                tileProps = GameController.arrayTile[tid];
                if (tileProps.pid != -1) {
                    var pieceProps = GameController.pidToPiece[tileProps.pid];
                    // return if colour doesn't match activeColour
                    switch (activeColour) {
                        case ActiveColour.White:
                            if (!pieceProps.isWhite) {
                                return false;
                            }

                            break;
                        case ActiveColour.Black:
                            if (pieceProps.isWhite) {
                                return false;
                            }
                            break;
                        default:
                            return false;
                    }
                    
                    ComputeDisplayMoves(tileProps);
                    movementState = MovementState.Selected;
                    _pidRef = tileProps.pid;
                    _tidRef = tid;
                }

                return false;

            case MovementState.Selected:
                if (_piece == null) {
                    Debug.Log("Something went wrong");
                    return false;
                }
                
                mousePos = Vector2Int.RoundToInt(GetMouseInfo());
                if (!ValidPos(mousePos)) {
                    return false;
                }
                
                tileProps = GameController.arrayTile[GameController.ToRawNum(mousePos)];
                Debug.Log("pid: " + tileProps.pid);
                int aPid = GameController.arrayTile[GameController.ToRawNum(mousePos)].pid;
                
                // handle case: user clicks on another piece of the same colour
                if (aPid != -1) {
                    var aPieceIsWhite = GameController.pidToPiece[aPid].isWhite;
                    Debug.Log("mouse white?: " + aPieceIsWhite + " selected white?: " + _piece.GetPieceProps().isWhite);
                    if (aPieceIsWhite == _piece.GetPieceProps().isWhite) {
                        DestroyUI();
                        movementState = MovementState.WaitSelect;
                        return false;
                    }
                }

                var pieceMoves = _piece.GetMoves();
                var pieceKills = _piece.GetKills();
                // var pieceProps = _piece.GetPieceProps();
                if (pieceMoves != null && pieceKills != null) {
                    DestroyUI();
                }
                
                // compare mousePos with list of moves
                if (pieceMoves != null) {
                    foreach (var move in pieceMoves) {
                            if (move.x != mousePos.x || move.y != mousePos.y) continue;
                            DestroyUI();
                            CommenceMove(mousePos);
                            movementState = MovementState.WaitSelect;
                            return true;
                    }
                }

                if (pieceKills != null) {
                    foreach (var kill in pieceKills) {
                        if (kill.Item2.x != mousePos.x || kill.Item2.y != mousePos.y) continue;

                        DestroyUI();

                        Destroy(GameController.physPieces[kill.Item1]);
                        Debug.Log(GameController.physPieces.Count);
                        
                        // generate array for range [16, 17, ...]
                        int physPieceCount = GameController.physPieces.Count;
                        int remaining = physPieceCount - (kill.Item1 + 1);
                        
                        // decrement corresponding pid's
                        int tileCount = GameController.arrayTile.Length;
                        int count = 0;
                        for (int i = 0; i < tileCount; i++) {
                            for (int j = 0; j < GameController.arrayTile.Length; j++) {
                                if (GameController.arrayTile[j].pid > kill.Item1) {
                                    GameController.arrayTile[j].pid--;
                                    count++;
                                }
                            }
                            if (count == remaining) {
                                Debug.Log(count);
                                break;
                            }
                        }

                        if (_pidRef > kill.Item1) {
                            _pidRef--; }
                        
                        GameController.physPieces.RemoveAt(kill.Item1);
                        GameController.pidToPiece.RemoveAt(kill.Item1);

                        // increment kill score etc
                        
                        CommenceMove(mousePos);
                        movementState = MovementState.WaitSelect;
                        return true;
                    }
                }
                
                // if its an empty tile and wasn't part of moves or kills, reset UI and state

                DestroyUI();
                movementState = MovementState.WaitSelect;
                return false;

            default:
                movementState = MovementState.WaitSelect;
                break;
        }
        return false;
    }

    private void CommenceMove(Vector2Int pos) {
        Debug.Log(_pidRef + " " + GameController.physPieces.Count);
        // change physical piece pos
        GameController.physPieces[_pidRef].transform.position =
            new Vector3(pos.x, pos.y, 0);

        // change virtual piece pos
        GameController.pidToPiece[_pidRef].tVec = pos;

        // set pid of old tile to -1
        int tmpPid = GameController.arrayTile[_tidRef].pid;
        GameController.arrayTile[_tidRef].pid = -1;

        // transfer pid to new tile
        int tid = GameController.ToRawNum(new Vector2(Mathf.Floor(pos.x),
            Mathf.Floor(pos.y)));
        GameController.arrayTile[tid].pid = tmpPid;
        
        // swap active colour
        activeColour = activeColour == ActiveColour.White ? ActiveColour.Black : ActiveColour.White;
    }
    
    private Vector2 GetMouseInfo() {
        Vector3 rawMousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.transform.position.z * -1);
        Vector2 mousePos = (Vector2)cam.ScreenToWorldPoint(rawMousePos);
        return mousePos;
    }

    private void ComputeDisplayMoves(TileProps tileProps) {
        Debug.Log(tileProps.ToString());

        // instantiate squareSelected for empty tiles as well?
        var tmpUI = CreateUI(squareSelected, tileProps.vec);
        _ui.Add(tmpUI);
        _piece = DecidePiece(tileProps.pid);
        _piece.MakeMoves(); 
        var moves = _piece.GetMoves();
        var kills = _piece.GetKills();
        
        // piece.OutputMoves();
        _piece.MakeHMoves();
        _piece.OutputHMoves();
        if (moves != null && moves.Length != 0) {
            foreach (var t in moves) {
                tmpUI = CreateUI(squareMovable, t);
                _ui.Add(tmpUI);
            }
        }

        if (kills == null || kills.Length == 0) return;
        foreach (var kill in kills) {
            tmpUI = CreateUI(squareKillable, kill.Item2);
            _ui.Add(tmpUI);
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

    private void DestroyUI() {
        foreach (var element in _ui) {
            Destroy(element);
        }
        _ui.Clear();
    }
    
    public static bool ValidPos(Vector2Int pos) {
        if (pos.x is < 0 or > 7) return false;
        return pos.y is >= 0 and <= 7;
    }
    
    // TODO: switch to below for efficiency
    public static bool ValidPos(int x, int y) {
        if (x is < 0 or > 7) return false;
        return y is >= 0 and <= 7;
    }

    IEnumerator WaitForUser() {
        if (!OnClick()) {
            yield return null; 
        }

        yield return new WaitForSeconds(0.5f);
    }
}
