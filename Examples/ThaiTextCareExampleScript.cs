using PhEngine.ThaiTextCare;
using TMPro;
using UnityEngine;

public class ThaiTextCareExampleScript : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;
    [SerializeField] TMP_Text outputText;
    [SerializeField] TMP_Text wordCountText;
    [SerializeField] ThaiTextNurse nurse;

    void Start()
    {
        inputField.onValueChanged.AddListener(OnValueChanged);
        RefreshWordCount();
    }

    void OnValueChanged(string input)
    {
        outputText.text = input;
        RefreshWordCount();
    }

    void RefreshWordCount()
    {
        wordCountText.text = nurse.LastWordCount.ToString("N0") + " Words";
    }
}