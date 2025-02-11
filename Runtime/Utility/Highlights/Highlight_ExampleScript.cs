using TMPro;
using UnityEngine;

namespace PhEngine.ThaiTextCare.Utility
{
    public class Highlight_ExampleScript : MonoBehaviour
    {
        [SerializeField] TMP_Text text;
        [SerializeField] WordHighlighter wordHighlighter;
        void Start()
        {
            wordHighlighter.OnWordHighlighted += OnWordHighlighted;
        }

        void OnWordHighlighted(Highlight obj)
        {
            Debug.Log("Add: " + obj.Word.word);
            var newText = text.text + obj.Word.word;
            text.SetText(newText);
        }
    }
}