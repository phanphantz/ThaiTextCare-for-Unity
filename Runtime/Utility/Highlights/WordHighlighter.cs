using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace PhEngine.ThaiTextCare.Utility
{
    public class WordHighlighter : MonoBehaviour
    {
        [Header("Settings")]
        public float maxDistance = 1f;
        public HighlightMode highlightMode;
        public DuplicateHighlightMode duplicateHighlightMode;
        
        [Header("Components")]
        public Camera targetCamera;
        public Highlight highlightPrefab;

        [Header("Runtime")] 
        [SerializeField] List<WordHit> wordHitList = new List<WordHit>();
        public IReadOnlyList<WordHit> WordHits => wordHitList.AsReadOnly();

        [SerializeField] List<Highlight> highlightList = new List<Highlight>();
        public IReadOnlyList<Highlight> Highlights => highlightList.AsReadOnly();

        public event Action<Highlight> OnWordHighlighted;
        public event Action<Highlight> OnDispose;
        
        public void HighlightWordFromMouse(TextMeshProUGUI text)
        {
            TryHighlightWordFromMouse(text, out _);
        }
        
        public bool TryHighlightWordFromMouse(TextMeshProUGUI text, out WordHit hit)
        {
            hit = null;
            var cam = this.targetCamera ?? Camera.main;
            if (!WordUtils.TryGetHitFromMouse(text, cam, out hit, maxDistance))
                return false;

            return TryAddHighlight(hit);
        }

        public bool TryAddHighlight(WordHit hit)
        {
            var other = hit;
            if (highlightMode == HighlightMode.Single)
                DisposeAll();
            
            var existingHighlights = duplicateHighlightMode == DuplicateHighlightMode.Allowed ? 
                new Highlight[] {} : GetExistingHighlightsOf(other);
            
            var isDuplicated = existingHighlights.Length != 0;
            if (duplicateHighlightMode == DuplicateHighlightMode.Blocked && isDuplicated)
                return false;

            if (duplicateHighlightMode == DuplicateHighlightMode.Remove && isDuplicated)
            {
                foreach (var highlight in existingHighlights)
                    RemoveHighlight(highlight);
                return false;
            }
            
            AddHighlight(hit);
            return true;
        }

        Highlight[] GetExistingHighlightsOf(WordHit wordHit)
        {
            return highlightList
                .Where(w => w.Word.IsSameAs(wordHit))
                .ToArray();
        }

        public Highlight AddHighlight(WordHit word)
        {
            if (highlightPrefab == null)
            {
                Debug.LogError("Highlight prefab is null");
                return null;
            }
            
            wordHitList.Add(word);
            var highlight = highlightPrefab.Clone(word);
            highlight.SetWord(word);
            highlight.PlaceAt(word);
            highlightList.Add(highlight);
            OnWordHighlighted?.Invoke(highlight);
            return highlight;
        }

        public void RemoveHighlight(Highlight highlight)
        {
            wordHitList.Remove(highlight.Word);
            highlightList.Remove(highlight);
            Dispose(highlight);
        }

        void Dispose(Highlight highlight)
        {
            OnDispose?.Invoke(highlight);
            highlight.Dispose();
        }

        [ContextMenu(nameof(DisposeAll))]
        public void DisposeAll()
        {
            foreach (var highlight in highlightList)
                Dispose(highlight);

            wordHitList.Clear();
            highlightList.Clear();
        }
    }

    public enum DuplicateHighlightMode
    {
        Blocked, Remove, Allowed
    }

    public enum HighlightMode
    {
        Append, Single
    }
}