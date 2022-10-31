using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;

public class EventManager : MonoBehaviour {
    public GameObject inputField;
    public GameObject errorLabel;

    private void Start() {
        errorLabel.SetActive(false);
    }

    public void FenSubmitted(string input) {
        if (string.IsNullOrWhiteSpace(input)) {
            FenUtility.FenToBoard("default");
            inputField.SetActive(false);
            FenUtility.fenCreated = true;
            return;
        }

        if (!FenUtility.FenToBoard(input)) {
            StartCoroutine(ShowHideError());
            return;
        }
        
        inputField.SetActive(false);
        FenUtility.fenCreated = true;
    }

    IEnumerator ShowHideError() {
        if (errorLabel.activeSelf) { yield break; }
        errorLabel.SetActive(true);
        yield return new WaitForSeconds(7);
        errorLabel.SetActive(false);
    }
}
