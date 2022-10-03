using System;
using UnityEngine;

public class PieceController : MonoBehaviour {
    private GameObject _selectedAss;
    public GameObject squareSelected;

    private bool _pieceSelected;

    private void Start() {
        _pieceSelected = false;
    }

    private void FixedUpdate() {
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
        // convert to 'real' pos
        mousePos.x += 0.5f;
        mousePos.y += 0.5f;
        try {
            Debug.Log(mousePos);
            Collider2D targetObject = Physics2D.OverlapPoint(mousePos);
            if (targetObject) {
                _selectedAss = targetObject.transform.gameObject;
                TileProps tileProps = _selectedAss.GetComponent<TileProps>();
                if (tileProps.pid != 0) {
                    Instantiate(squareSelected, new Vector2(Mathf.Floor(mousePos.x), Mathf.Floor(mousePos.y)),
                        Quaternion.identity);
                    _pieceSelected = true;
                }
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
