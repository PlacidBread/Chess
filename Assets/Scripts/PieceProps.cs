using System;
using UnityEngine;

public class PieceProps{
    public Vector2Int tVec;
    
    public PieceType type;
    public bool isWhite;

    // public PieceProps this[int index] {
    //     get => GameController.pidToPiece[index];
    // }
    public void OutputInfo() {
        Debug.Log("PieceProps - " + ToString());
    }
    public override string ToString() {
        return ("vec: " + tVec + ", type: " + type + ", colour: " + (isWhite ? "white" : "black"));
    }
}
