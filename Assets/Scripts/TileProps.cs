using System;
using UnityEngine;

public class TileProps {
    public int id;
    [NonSerialized]
    public Tuple<char, int> pos;
    public Vector2Int vec;
    
    public int pid; // corresponding piece ID (-1 if none)
    
    public void OutputInfo() {
        Debug.Log("TileProps - " + ToString());
    }

    public override string ToString() {
        return ("id: " + id + ", pid: " + pid + ", vec: " + vec);
    }
}
