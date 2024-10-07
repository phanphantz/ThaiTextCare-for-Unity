using PhEngine.ThaiTextCare;
using TMPro;
using UnityEngine;

public class ThaiTextCare_ExampleScript : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;
    [SerializeField] TMP_InputField separatorInputField;
    [SerializeField] TMP_Text outputText;
    [SerializeField] TMP_Text wordCountText;
    [SerializeField] ThaiTextNurse nurse;

    void Start()
    {
        nurse.OnTokenized += result => RefreshWordCount(result.WordCount);
        inputField.onValueChanged.AddListener(OnOriginalMessageChanged);
        separatorInputField.onValueChanged.AddListener(OnSeparatorChanged);
    }

    void OnOriginalMessageChanged(string input)
    {
        outputText.text = input;
    }

    void OnSeparatorChanged(string value)
    {
        nurse.Separator = value;
    }

    void RefreshWordCount(int count)
    {
        wordCountText.text = count.ToString("N0") + " Words";
    }
}