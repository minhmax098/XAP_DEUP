using System.Collections;
using System.Collections.Generic;
using AdvancedInputFieldPlugin;
using UnityEngine;
using UnityEngine.UI; 

public class InputFieldReset : MonoBehaviour
{
    public Button resetBtn; 
    public AdvancedInputField inputField;
    void Start()
    {
        resetBtn.gameObject.SetActive(false);
        resetBtn.onClick.AddListener(ResetInputFieldValue); 
        inputField.OnValueChanged.AddListener(OnInputFieldValueChanged);
    }
    void ResetInputFieldValue()
    {
        inputField.Text = "";
        inputField.Select();
        resetBtn.gameObject.SetActive(false);
    }

    void OnInputFieldValueChanged(string data)
    {
        resetBtn.gameObject.SetActive(!string.IsNullOrEmpty(data));
    }
}
