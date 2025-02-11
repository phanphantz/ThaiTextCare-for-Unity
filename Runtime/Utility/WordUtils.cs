using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

namespace PhEngine.ThaiTextCare.Utility
{
    public static class WordUtils
    {
        public static bool TryGetHitFromMouse(TextMeshProUGUI targetText, out WordHit request, float maxDistance = 1f, char[] customSeparators = null)
            => TryGetHit(targetText, Input.mousePosition, Camera.main, out request, maxDistance, customSeparators);
        
        public static bool TryGetHitFromMouse(TextMeshProUGUI targetText, Camera camera, out WordHit request, float maxDistance = 1f, char[] customSeparators = null)
            => TryGetHit(targetText, Input.mousePosition, camera, out request, maxDistance, customSeparators);
        
        public static bool TryGetHit(TextMeshProUGUI targetText, Vector3 position,  out WordHit request, float maxDistance = 1f, char[] customSeparators = null)
            => TryGetHit(targetText, position, Camera.main, out request, maxDistance, customSeparators);
        
        public static bool TryGetHit(TextMeshProUGUI targetText, Vector3 position, Camera camera, out WordHit hit, float maxDistance = 1f, char[] customSeparators = null)
        {
            hit = null;
            var nearestCharacterIndex = FindNearestCharacter(targetText, position, camera, false, maxDistance);
            if (nearestCharacterIndex == -1)
                return false;
            
            return GetWordFromCharacterInfoIndex(targetText, nearestCharacterIndex, out hit, customSeparators);
        }

        public static bool GetWordFromCharacterInfoIndex(TextMeshProUGUI targetText, int characterInfoIndex, out WordHit hit, char[] customSeparators = null)
        {
            hit = null;
            var characters = targetText.textInfo.characterInfo;
            var nearestCharacter = characters[characterInfoIndex].character;
            if (IsEndOfWordOrSpace(nearestCharacter, customSeparators))
                return false;
            
            var startIndex = FindNearestSeparator(characters, characterInfoIndex, -1, customSeparators);
            var endIndex = FindNearestSeparator(characters, characterInfoIndex, 1, customSeparators);
            if (startIndex < 0 || endIndex >= characters.Length)
                return false;

            var startCharacter = characters[startIndex];
            var endCharacter = characters[endIndex];

            //If the end character is on a different line, use last visible character on the same line instead.
            if (endCharacter.lineNumber != startCharacter.lineNumber)
            {
                endIndex = targetText.textInfo.lineInfo[startCharacter.lineNumber].lastVisibleCharacterIndex;
                endCharacter = characters[endIndex];
            }
            
            var startPosition = startCharacter.bottomLeft;
            startPosition.y = startCharacter.baseLine;
            var endPosition = endCharacter.bottomRight;
            endPosition.y = startCharacter.baseLine;
            
            var word = GetWord(characters, startIndex, endIndex);
            hit = new WordHit(targetText, word, nearestCharacter, startPosition, endPosition, endIndex, startIndex);
            return true;
        }
        
        static int FindNearestCharacter(TextMeshProUGUI text, Vector3 position, Camera camera, bool visibleOnly, float maxDistance)
        {
            RectTransform rectTransform = text.rectTransform;

            float distanceSqr = Mathf.Infinity;
            int closest = 0;

            // Convert position into Worldspace coordinates
            TMP_TextUtilities.ScreenPointToWorldPointInRectangle(rectTransform, position, camera, out position);

            for (int i = 0; i < text.textInfo.characterCount; i++)
            {
                // Get current character info.
                TMP_CharacterInfo cInfo = text.textInfo.characterInfo[i];
                if (visibleOnly && !cInfo.isVisible) continue;

                // Get Bottom Left and Top Right position of the current character
                Vector3 bl = rectTransform.TransformPoint(cInfo.bottomLeft);
                Vector3 tl = rectTransform.TransformPoint(new Vector3(cInfo.bottomLeft.x, cInfo.topRight.y, 0));
                Vector3 tr = rectTransform.TransformPoint(cInfo.topRight);
                Vector3 br = rectTransform.TransformPoint(new Vector3(cInfo.topRight.x, cInfo.bottomLeft.y, 0));

                if (PointIntersectRectangle(position, bl, tl, tr, br))
                    return i;

                // Find the closest corner to position.
                float dbl = TMP_TextUtilities.DistanceToLine(bl, tl, position);
                float dtl = TMP_TextUtilities.DistanceToLine(tl, tr, position);
                float dtr = TMP_TextUtilities.DistanceToLine(tr, br, position);
                float dbr = TMP_TextUtilities.DistanceToLine(br, bl, position);

                float d = dbl < dtl ? dbl : dtl;
                d = d < dtr ? d : dtr;
                d = d < dbr ? d : dbr;

                if (distanceSqr > d)
                {
                    distanceSqr = d;
                    closest = i;
                }
            }

            if (distanceSqr > (maxDistance * maxDistance))
                return -1;

            return closest;
        }
        
        static bool PointIntersectRectangle(Vector3 m, Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            Vector3 ab = b - a;
            Vector3 am = m - a;
            Vector3 bc = c - b;
            Vector3 bm = m - b;

            float abamDot = Vector3.Dot(ab, am);
            float bcbmDot = Vector3.Dot(bc, bm);

            return 0 <= abamDot && abamDot <= Vector3.Dot(ab, ab) && 0 <= bcbmDot && bcbmDot <= Vector3.Dot(bc, bc);
        }

        static string GetWord(TMP_CharacterInfo[] characters, int startIndex, int endIndex)
        {
            var result = new StringBuilder();
            for (int i = startIndex; i <= endIndex; i++)
                result.Append(characters[i].character);
            
            return result.ToString().Trim();
        }

        static bool IsEndOfWordOrSpace(char currentChar, char[] customSeparators = null)
        {
            return currentChar.ToString() is "​" or "\t" or " " or "\n" or "\r" or "\r\n" 
                   || (customSeparators != null && customSeparators.Contains(currentChar));
        }

        static int FindNearestSeparator(TMP_CharacterInfo[] message, int startIndex, int direction, char[] customSeparators = null)
        {
            var index = startIndex + direction;
            if (index < 0)
                return 0;

            if (index >= message.Length)
                return message.Length - 1;

            var currentChar = message[index];
            if (IsEndOfWordOrSpace(currentChar.character, customSeparators))
                return index - direction;

            return FindNearestSeparator(message, index, direction, customSeparators);
        }
    }
}