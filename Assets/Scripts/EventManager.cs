using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;

public class EventManager : MonoBehaviour {
    public GameObject inputField;
    public GameObject errorLabel;
    public GameObject defaultButton;

    private TMP_InputField _inputText;
    private void Start() {
        _inputText = GameObject.Find("FenInput").GetComponent<TMP_InputField>();
        Debug.Log(_inputText);
        errorLabel.SetActive(false);
    }

    public void FenSubmitted(string input) {    
        if (string.IsNullOrWhiteSpace(input)) {
            // FenUtility.FenToBoard("default");
            // inputField.SetActive(false);
            // FenUtility.fenCreated = true;
            return;
        }
        // call separate / same function with 'on select'
        if (!FenUtility.FenToBoard(input)) {
            StartCoroutine(ShowHideStartError());
            return;
        }
        
        HideStartUI();
        FenUtility.fenCreated = true;
        StartCoroutine(DelayGameStart());
    }

    IEnumerator ShowHideStartError() {
        if (errorLabel.activeSelf) { yield break; }
        errorLabel.SetActive(true);
        yield return new WaitForSeconds(7);
        if (errorLabel.activeSelf) { errorLabel.SetActive(false); }
    }

    private void HideStartUI() {
        inputField.SetActive(false);
        defaultButton.SetActive(false);
        if (errorLabel.activeSelf) { errorLabel.SetActive(false); }
    }
    
    // allow everything to render etc.
    IEnumerator DelayGameStart() {
        yield return new WaitForSeconds(0.5f);
        PieceController.movementState = MovementState.WaitSelect;
    }

    public void GenDefaultFen() {
        _inputText.text = FenUtility.StartFen;
    }
}
