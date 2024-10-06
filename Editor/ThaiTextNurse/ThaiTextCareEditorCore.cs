using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PhEngine.ThaiTextCare.Editor
{
    [InitializeOnLoad]
    public static class ThaiTextCareEditorCore 
    {
        static ThaiTextCareEditorCore()
        {
            var settings = ThaiTextCareSettings.PrepareInstance();
            if (settings && settings.IsLoadDictionaryOnEditorStartUp)
                ThaiTextNurse.RebuildDictionary();
        }

        public static void AddWordsToDictionary(string pendingWords)
        {
            var settings = ThaiTextCareSettings.PrepareInstance();
            if (!ThaiTextNurse.TryLoadDictionaryAsset(settings, out var textAsset))
                return;

            var assetPath = AssetDatabase.GetAssetPath(textAsset);
            var inputWords = GetWords(pendingWords);
            var succeedList = new List<string>();
            var dictionaryWordList = new List<string>(ThaiTextNurse.WordsFromDictionary(textAsset));
            foreach (var word in inputWords)
            {
                if (!dictionaryWordList.Contains(word))
                {
                    dictionaryWordList.Add(word);
                    succeedList.Add(word);
                }
            }
            var content = string.Join(System.Environment.NewLine, dictionaryWordList);
            SaveDictionaryAndRebuild(assetPath, content);
            var message = succeedList.Count == 1 ? "Added '" + succeedList.FirstOrDefault() + "'" : "Added " + succeedList.Count + " New Words";
            Debug.Log("Added: " + string.Join(", ", succeedList));
            SceneView.lastActiveSceneView.ShowNotification(new GUIContent(message), 1f);
        }

        static void SaveDictionaryAndRebuild(string assetPath, string content)
        {
            File.WriteAllText(assetPath, content); // Save as a plain text file
            AssetDatabase.Refresh();
            ThaiTextNurse.RebuildDictionary();
        }

        static string[] GetWords(string input)
        {
            return input.Split(' ').Select(w => w.Trim()).ToArray();
        }

        public static void RemoveWordsFromDictionary(string pendingWords)
        {
            var settings = ThaiTextCareSettings.PrepareInstance();
            if (!ThaiTextNurse.TryLoadDictionaryAsset(settings, out var textAsset))
                return;

            var assetPath = AssetDatabase.GetAssetPath(textAsset);
            var inputWords = GetWords(pendingWords);
            var dictionaryWordList = new List<string>(ThaiTextNurse.WordsFromDictionary(textAsset));
            var succeedList = new List<string>();
            foreach (var word in inputWords)
            {
                if (dictionaryWordList.Remove(word))
                    succeedList.Add(word);
            }
            var content = string.Join(System.Environment.NewLine, dictionaryWordList);
            SaveDictionaryAndRebuild(assetPath, content);
            var message = succeedList.Count == 1 ? "Removed '" + succeedList.FirstOrDefault() + "'" : "Removed " + succeedList.Count + " Words";
            Debug.Log("Removed: " + string.Join(", ", succeedList));
            SceneView.lastActiveSceneView.ShowNotification(new GUIContent(message), 1f);
        }
    }
}