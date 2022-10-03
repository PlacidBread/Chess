using System;
using UnityEngine;

public class TileProps : MonoBehaviour {
    // use get / set ?
    public int id;
    [NonSerialized]
    public int[] pos;
    public Vector2 vec;
    
    public int pid; // corresponding piece ID if applicable

    public void OutputInfo() {
        Debug.Log(id + ": " + vec);
    }
}
