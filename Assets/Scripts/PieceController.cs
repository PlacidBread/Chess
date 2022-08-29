using System;
using UnityEngine;

public class PieceController : MonoBehaviour {
    public Camera cam;

    private void Start() {
        
    }

    private void Update() {
        Vector3 mousePos;
        try {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        catch (System.NullReferenceException e) {
            Debug.Log("No mouse.");
            // handle 'no mouse' UI message
        }
        
    }

}
