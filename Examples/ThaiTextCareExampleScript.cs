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
        nurse.OnTokenized += result => RefreshWordCount(result.WordCount);
        inputField.onValueChanged.AddListener(OnValueChanged);
    }

    void OnValueChanged(string input)
    {
        outputText.text = input;
    }

    void RefreshWordCount(int count)
    {
        wordCountText.text = count.ToString("N0") + " Words";
    }
}