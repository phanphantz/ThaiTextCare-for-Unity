using System;
using TMPro;
using UnityEngine;

namespace PhEngine.ThaiTextCare.Utility
{
    [Serializable]
    public class WordHit
    {
        public WordHit(TMP_Text text, string word, char nearestCharacter, Vector3 startPosition, Vector3 endPosition, int endIndex, int startIndex)
        {
            this.nearestCharacter = nearestCharacter;
            this.word = word;
            this.text = text;
            fontSize = this.text.fontSize;
            this.startPosition = startPosition;
            this.endPosition = endPosition;
            this.startIndex = startIndex;
            this.endIndex = endIndex;
        }

        public string word;
        public char nearestCharacter;
        public TMP_Text text;
        public float fontSize;
        
        public float Width => endPosition.x - startPosition.x;
        
        [Header("Start")]
        public int startIndex;
        public Vector3 startPosition;
        
        [Header("End")]
        public int endIndex;
        public Vector3 endPosition;

        public bool IsSameAs(WordHit other)
        {
            return text == other.text 
                    && startIndex == other.startIndex
                    && endIndex == other.endIndex
                    && word == other.word;
        }
    }
}