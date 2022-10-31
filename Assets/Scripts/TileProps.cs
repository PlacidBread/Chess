using System;
using UnityEngine;

public class TileProps {
    // use get / set ?
    public int id;
    [NonSerialized]
    public Tuple<char, int> pos;
    public Vector2Int vec;
    
    public int pid; // corresponding piece ID if applicable
    
    public void OutputInfo() {
        Debug.Log(id + ": " + vec);
    }

    public override string ToString() {
        return ("id: " + id + ", pid: " + pid + ", vec: " + vec);
    }
}
