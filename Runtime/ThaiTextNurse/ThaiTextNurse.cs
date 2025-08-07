using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PhEngine.ThaiTextCare
{
    [RequireComponent(typeof(TMP_Text)), ExecuteAlways]
    public class ThaiTextNurse : MonoBehaviour, ITextPreprocessor
    {
        public ThaiGlyphCorrection Correction
        {
            get => correction;
            set
            {
                correction = value;
                NotifyChange();
            }
        }
        [SerializeField] ThaiGlyphCorrection correction;

        public bool IsForceFullLineHeight
        {
            get => isForceFullLineHeight;
            set
            {
                isForceFullLineHeight = value;
                NotifyChange();
            }
        }
        
        [Tooltip("Force inject <line-height=100%> tag to the output string to fix the issue where line spacing increase as you modify the Glyph adjusment Y offset.")]
        [SerializeField] bool isForceFullLineHeight;
        
        public bool IsTokenize
        {
            get => isTokenize;
            set
            {
                isTokenize = value;
                NotifyChange();
            }
        }
        [SerializeField] bool isTokenize = true;
        
        public string Separator
        {
            get => separator;
            set
            {
                separator = value;
                NotifyChange();
            }
        }
        [SerializeField] string separator;

        public TMP_Text TextComponent => tmpText;
        [SerializeField] TMP_Text tmpText;
        [SerializeField, HideInInspector] string lastKnownText;
        
        public string OutputString => outputString;
        [SerializeField, HideInInspector] string outputString;
        
        public int LastWordCount => lastWordCount;
        public int CharacterInfoLength => tmpText.textInfo.characterInfo.Length;
        public static bool IsDictionaryLoaded { get; private set; }

        [SerializeField, HideInInspector] int lastWordCount;
        
        public WordBreakGUIMode guiMode;
        public Color guiColor = new Color(0f, 0.5f, 0.8f);
        
        static PhTokenizer tokenizer;
        bool isRebuildRequired;
        [SerializeField, HideInInspector] bool isInitialized;

        public event Action<TokenizeResult> OnTokenized;

        void Awake()
        {
            if (tmpText == null)
                tmpText = GetComponent<TMP_Text>();
            isRebuildRequired = true;
#if UNITY_EDITOR
            if (!isInitialized)
            {
                while (UnityEditorInternal.ComponentUtility.MoveComponentUp(this)){}
                isInitialized = true;
            }
#endif
        }

        void OnEnable()
        {
            tmpText.textPreprocessor = this;
        }

        void OnDisable()
        {
            tmpText.textPreprocessor = null;
        }

        void OnDestroy()
        {
            tmpText.textPreprocessor = null;
        }
        
        public void NotifyChange()
        {
            isRebuildRequired = true;
            tmpText.havePropertiesChanged = true;
        }
        
        public string PreprocessText(string text)
        {
            if (lastKnownText == text && !isRebuildRequired)
                return outputString;
            
            lastKnownText = text;
            isRebuildRequired = false;
            
            //Sanity Check
            //Debug.Log(gameObject.name + " : Rebuild Display String");
            return RebuildOutputString(text);
        }

        string RebuildOutputString(string text)
        {
            outputString = text;
            if (isTokenize)
                outputString = Tokenize();
            
            if (correction != ThaiGlyphCorrection.None)
                outputString = ThaiFontAdjuster.Adjust(outputString, correction);

            if (isForceFullLineHeight)
                return $"<line-height=100%>{outputString}</line-height>";
            else
                return outputString;
        }

        string Tokenize()
        {
            var request = new TokenizeRequest(outputString, separator, true, tmpText.richText);
            if (TryTokenize(request, out var result))
            {
                lastWordCount = result.WordCount;
                outputString = result.Result;
                OnTokenized?.Invoke(result);
            }
            return outputString;
        }
        
        TMP_CharacterInfo GetCharacterInfo(int index)
        {
            return tmpText.textInfo.characterInfo[index];
        }

        bool IsShouldDrawGizmos()
        {
            if (guiMode == WordBreakGUIMode.Off || !isTokenize || !enabled || !tmpText.enabled)
                return false;

            if (string.IsNullOrEmpty(tmpText.text))
                return false;
            
            return true;
        }

        void OnDrawGizmos()
        {
            if (guiMode == WordBreakGUIMode.Always)
                VisualizeInEditor(this);
        }

        #region Static Methods
        
        public static string SafeTokenize(string input)
        {
            return SafeTokenize(new TokenizeRequest(input));
        }

        public static string SafeTokenize(TokenizeRequest request)
        {
            return TryTokenize(request, out var result) ? result.Result : request.Input;
        }
        
        public static bool TryTokenize(string input, out TokenizeResult result)
        {
            return TryTokenize(new TokenizeRequest(input), out result);
        }
        
        public static bool TryTokenize(TokenizeRequest tokenizeRequest, out TokenizeResult result)
        {
            result = null;
            var settings = ThaiTextCareSettings.PrepareInstance();
            if (tokenizer == null && !TryRebuildDictionary(settings))
                return false;
            
            if (tokenizer == null)
                return false;
            
            var wordBreakCharacter = GetWordBreakCharacter(settings);
            var finalSeparator = tokenizeRequest.Separator;
            if (tokenizeRequest.IsBreakWords)
                finalSeparator += wordBreakCharacter;
           
            if (string.IsNullOrEmpty(finalSeparator))
                return false;

            //Remove all existing word break characters
            var input = tokenizeRequest.Input;
            if (input.Contains(wordBreakCharacter))
                input = input.Replace(wordBreakCharacter, string.Empty);
            
            var words = tokenizer.Tokenize(input,  tokenizeRequest.IsSupportRichText);
            result = new TokenizeResult(string.Join(finalSeparator, words), words.Count(w => !string.IsNullOrEmpty(w.Trim())));
            return true;
        }

        public static string GetWordBreakCharacter(ThaiTextCareSettings settings)
        {
            return settings ? settings.WordBreakCharacter : "​";
        }

        public static void RebuildDictionary(bool isUpdateNursesInScene = true)
        {
#if UNITY_EDITOR
            EditorUtility.DisplayProgressBar("Rebuilding Dictionary", "Setting up the dictionary...", 0.25f);
#endif
            try
            {
                if (!TryRebuildDictionary(ThaiTextCareSettings.PrepareInstance()))
                {
#if UNITY_EDITOR
                    EditorUtility.ClearProgressBar();
#endif
                    throw new InvalidOperationException("Failed to setup Dictionary for ThaiTextNurse");
                }
#if UNITY_EDITOR
                EditorUtility.DisplayProgressBar("Rebuilding Dictionary", "Dictionary setup completed", 0.9f);
#endif
                if (isUpdateNursesInScene)
                {
#if UNITY_EDITOR
                    EditorUtility.DisplayProgressBar("Rebuilding Dictionary", "Updating nurses...", 0.95f);
#endif 
                    UpdateAllNursesInScene();
                }
            }
            finally
            {
#if UNITY_EDITOR
                EditorUtility.ClearProgressBar();
#endif
            }
        }

        public static void UpdateAllNursesInScene()
        {
            var existingNurses = FindObjectsOfType<ThaiTextNurse>();
            foreach (var nurse in existingNurses)
                nurse.NotifyChange();
        }
        
        public static void EnableAllTokenizerInScene()
        {
            var existingNurses = FindObjectsOfType<ThaiTextNurse>();
            foreach (var nurse in existingNurses)
                nurse.IsTokenize = true;
        }

        public static void DisableAllTokenizerInScene()
        {
            var existingNurses = FindObjectsOfType<ThaiTextNurse>();
            foreach (var nurse in existingNurses)
                nurse.IsTokenize = false;
        }

        public static IEnumerator RebuildDictionaryAsync(bool isUpdateNursesInScene = true, Action<float> onProgress = null, Action onFail = null)
        {
            var settings = ThaiTextCareSettings.PrepareInstance();
            var path = GetDictionaryPath(settings);
            var request = Resources.LoadAsync<TextAsset>(path);
            while (!request.isDone)
            {
                onProgress?.Invoke(request.progress);
                yield return new WaitForEndOfFrame();
            }
            
            var textAsset = request.asset as TextAsset;
            if (textAsset == null)
            {
                Debug.LogError("Cannot find any dictionary under Resources Folder path : " + path);
                onFail?.Invoke();
                yield break;
            }
            RebuildTokenizer(textAsset);
            if (isUpdateNursesInScene)
                UpdateAllNursesInScene();
        }

        static bool TryRebuildDictionary(ThaiTextCareSettings settings)
        {
            IsDictionaryLoaded = false;
            if (!TryLoadDictionaryAsset(settings, out var textAsset)) 
                return false;
            
            RebuildTokenizer(textAsset);
            return true;
        }

        public static bool TryLoadDictionaryAsset(ThaiTextCareSettings settings, out TextAsset textAsset)
        {
            var path = GetDictionaryPath(settings);
            textAsset = Resources.Load<TextAsset>(path);
            if (textAsset == null)
            {
                Debug.LogError("Cannot find any dictionary under Resources Folder path : " + path);
                return false;
            }
            return true;
        }

        public static string GetDictionaryPath(ThaiTextCareSettings settings)
        {
            return settings? settings.DictionaryResourcePath : "dictionary";
        }

        static void RebuildTokenizer(TextAsset textAsset)
        {
            tokenizer = new PhTokenizer(WordsFromDictionary(textAsset));
            Resources.UnloadAsset(textAsset);
            IsDictionaryLoaded = true;
            Debug.Log("[ThaiTextNurse] Dictionary Rebuild Completed!");
        }

        public static string[] WordsFromDictionary(TextAsset textAsset)
        {
            var content = textAsset.text;
            
            // Don't trust the file, Normalize all new lines to '\n'
            content = content.Replace("\r\n", "\n").Replace("\r", "\n");
            
            //Ignore empty and trim all words
            return content
                    .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(w => w.Trim())
                    .Where(w => !w.StartsWith('/'))
                    .ToArray();
        }
        
        public static void VisualizeInEditor(ThaiTextNurse nurse)
        {
#if UNITY_EDITOR
            if (!nurse.IsShouldDrawGizmos())
                return;
            
            var settings = ThaiTextCareSettings.PrepareInstance();
            var breakCharacter = ThaiTextNurse.GetWordBreakCharacter(settings);
            if (string.IsNullOrEmpty(breakCharacter))
                return;

            var breakIndices = new List<int>();
            for (int i = 0; i < nurse.CharacterInfoLength; i++)
            {
                //This is needed because CharacterInfos are kind of unreliable on undo
                if (i > nurse.outputString.Length)
                    break;
                
                if (nurse.GetCharacterInfo(i).character == breakCharacter[0])
                    breakIndices.Add(i);
            }

            var oldColor = Handles.color;
            var color = nurse.guiColor;
            color.a = 0.75f;
            Handles.color = color;
            var widthScale = nurse.transform.lossyScale.x;

            // 0.1f seems to be a magic number that makes the height scale looks correct for Worldspace texts.
            // Why? I don't know... Unity magic?
            var heightScale = nurse.transform.lossyScale.y * (nurse.TextComponent is TextMeshProUGUI ? 1f : 0.1f);
            foreach (int index in breakIndices)
            {
                var charInfo = nurse.GetCharacterInfo(index);
                Vector3 pos = nurse.transform.TransformPoint(charInfo.bottomRight);
                float lineHeight = charInfo.pointSize * heightScale;
                Handles.DrawLine(pos, pos + (nurse.transform.up * lineHeight), 3f * widthScale);
            }
            Handles.color = oldColor;
#endif
        }
        
        #endregion
    }

    public enum ThaiGlyphCorrection
    {
        None, YoorYingAndToorTaan, FullC90
    }

    public enum WordBreakGUIMode
    {
        OnSelected, Always, Off
    }
}