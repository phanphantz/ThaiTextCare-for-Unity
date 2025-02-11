using TMPro;
using UnityEngine;

namespace PhEngine.ThaiTextCare.Utility
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class Highlightable : MonoBehaviour
    {
        [SerializeField, HideInInspector] TextMeshProUGUI text;
        void Awake()
        {
            text = GetComponent<TextMeshProUGUI>();
        }

        public bool HighlightBy(WordHighlighter highlighter)
        {
            return highlighter.TryHighlightWordFromMouse(text, out _);
        }
    }
}