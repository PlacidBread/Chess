using System;
using UnityEngine;

public class PieceController : MonoBehaviour {
    private GameObject _selectedAss;
    
    public GameObject squareSelected;
    public GameObject darkSquare;
    public GameObject lightSquare;
    
    private bool _pieceSelected;

    private void Start() {
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
        Vector2 mousePos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.x += (float)0.001;
        try {
            Collider2D targetObject = Physics2D.OverlapPoint(mousePos);
            if (targetObject) {
                _selectedAss = targetObject.transform.gameObject;
                Debug.Log(_selectedAss);
                
                // TileProps tileProps = _selectedAss.GetComponent<TileProps>();
                // if (tileProps.pid != 0) {
                //     Instantiate(squareSelected, new Vector2(Mathf.Floor(mousePos.x), Mathf.Floor(mousePos.y)),
                //         Quaternion.identity);
                //     // _pieceSelected = true;
                // }
            }
        }
        catch (NullReferenceException e) {
            Debug.Log("No mouse attached: " + e.Message);
            // handle 'no mouse' UI message
        }
    }

    // private bool validPos(Vector2 pos) {
    //     if (pos.x is < 0 or > 8) return false;
    //     return pos.y is >= 0 and <= 8;
    // }
}
