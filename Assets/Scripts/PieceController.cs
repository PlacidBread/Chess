using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class PieceController : MonoBehaviour {
    private GameObject selectedPiece;
    public GameObject squareSelected;

    private bool _isHighlighted;
    private void Start() {}

    private void Update() {
        Vector2 mousePos;
        if (Input.GetMouseButtonDown(0)) {
            try {
                mousePos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
                // convert to 'real' pos
                mousePos.x += 0.5f;
                mousePos.y += 0.5f;
                Debug.Log(mousePos);
                Collider2D targetObject = Physics2D.OverlapPoint(mousePos);
                if (targetObject) {
                    selectedPiece = targetObject.transform.gameObject;
                    Instantiate(squareSelected, new Vector2(Mathf.Floor(mousePos.x), Mathf.Floor(mousePos.y)), Quaternion.identity);
                    // Instantiate(squareSelected, selectedPiece.transform.position, Quaternion.identity);
                    Debug.Log(selectedPiece);
                }
                
                // TODO: If piece selected, select square to move to
            }
            catch (NullReferenceException e) {
                Debug.Log("No mouse attached: " + e.Message);
                // handle 'no mouse' UI message
            }
        }
    }

    private bool validPos(Vector2 pos) {
        if (pos.x is < 0 or > 8) return false;
        return pos.y is >= 0 and <= 8;
    }
}
