using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PhEngine.ThaiTextCare.Editor
{
    [CustomEditor(typeof(ThaiTextNurse), true), CanEditMultipleObjects]
    public class ThaiTextNurseEditor : UnityEditor.Editor
    {
        ThaiTextNurse nurse;
        GUIStyle miniTextStyle;
        string pendingWords;

        void OnEnable()
        {
            nurse = target as ThaiTextNurse;
        }

        public override void OnInspectorGUI()
        {
            miniTextStyle = new GUIStyle(EditorStyles.miniBoldLabel);
            miniTextStyle.alignment = TextAnchor.MiddleRight;

            PropertyField("correction");
            var settings = ThaiTextCareSettings.PrepareInstance();
            if (settings == null)
                EditorGUILayout.HelpBox("ThaiTextCareSettings.asset is missing. It must be under: " + ThaiTextCareSettings.SettingsPath , MessageType.Error);
            else if (!ThaiTextNurse.IsDictionaryLoaded)
            {
                EditorGUILayout.HelpBox("Dictionary is not loaded. It must be under 'Resources' folder: " + ThaiTextNurse.GetDictionaryPath(settings) , MessageType.Error);
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                var isRefresh = GUILayout.Button("Refresh", GUILayout.ExpandWidth(false));
                EditorGUILayout.EndHorizontal();
                if (isRefresh)
                    ThaiTextNurse.RebuildDictionary();
            }
            
            EditorGUI.BeginDisabledGroup(!ThaiTextNurse.IsDictionaryLoaded || settings == null);
            var currentText = nurse.OutputString;
            var correction = (ThaiGlyphCorrection) serializedObject.FindProperty("correction").enumValueIndex;
            if (correction  == ThaiGlyphCorrection.YoorYingAndToorTaan)
            {
                if (!currentText.Contains("ญ") && !currentText.Contains("ฐ"))
                {
                    EditorGUILayout.HelpBox("Your text does not contain any ญ or ฐ. You probably don't need this", MessageType.Warning);
                }
            }
            if (correction != ThaiGlyphCorrection.None)
            {
                if (currentText.Any(c => !nurse.TextComponent.font.HasCharacter(c)))
                {
                    EditorGUILayout.HelpBox("Your Font is missing some extended glyphs in C90 Encoding. Use FontForge to fix this", MessageType.Error);
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("How?", GUILayout.ExpandWidth(false)))
                        Application.OpenURL("https://fontforge.org/en-US/");
                    EditorGUILayout.EndHorizontal();
                }
            }
            
            PropertyField("isForceFullLineHeight");
            
            EditorGUILayout.BeginHorizontal();
            PropertyField("isTokenize");
            if (serializedObject.FindProperty("isTokenize").boolValue)
            {
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField(Selection.count == 1 ? nurse.LastWordCount.ToString("N0") + " Words" : "- Words", miniTextStyle);
            }

            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel++;
            PropertyField("separator");
            EditorGUI.indentLevel--;
            DrawCopyToClipboardButton();

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Editor-Only", EditorStyles.boldLabel);
            ThaiTextCareGUI.DrawHorizontalLine();
            PropertyField("guiMode");
            PropertyField("guiColor");
            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                foreach (var t in targets)
                    (t as ThaiTextNurse)?.NotifyChange();
            }

            EditorGUILayout.Space();
            if (DrawDictionarySection())
                return;
            
            EditorGUILayout.EndVertical();
            EditorGUI.EndDisabledGroup();

            ThaiTextCareGUI.DrawBugReportButton();
        }

        void DrawCopyToClipboardButton()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Copy Output to Clipboard", GUILayout.ExpandWidth(false)))
            {
                GUIUtility.systemCopyBuffer = nurse.OutputString;
                SceneView.lastActiveSceneView.ShowNotification(new GUIContent("Copied Output to Clipboard"), 1f);
            }
            EditorGUILayout.EndHorizontal();
        }

        bool DrawDictionarySection()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("\u2192", EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
            {
                if (ThaiTextNurse.TryLoadDictionaryAsset(ThaiTextCareSettings.PrepareInstance(), out var textAsset))
                {
                    Selection.SetActiveObjectWithContext(textAsset, textAsset);
                    EditorGUIUtility.PingObject(textAsset);
                }
            }
            EditorGUILayout.LabelField("Dictionary",  EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Force Rebuild", EditorStyles.linkLabel, GUILayout.ExpandWidth(false)))
            {
                ThaiTextNurse.RebuildDictionary();
                return true;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel++;
            pendingWords = EditorGUILayout.TextField("Words :", pendingWords);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Use [Space] to separate words", EditorStyles.miniBoldLabel);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Remove", GUILayout.ExpandWidth(false)))
            {
                if (!string.IsNullOrEmpty(pendingWords))
                    ThaiTextCareEditorCore.RemoveWordsFromDictionary(pendingWords.Trim());
                return true;
            }

            if (GUILayout.Button("Add", GUILayout.ExpandWidth(false)))
            {
                if (!string.IsNullOrEmpty(pendingWords))
                    ThaiTextCareEditorCore.AddWordsToDictionary(pendingWords.Trim());
                return true;
            }

            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
            return false;
        }

        void PropertyField(string fieldName)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(fieldName));
        }

        void OnSceneGUI()
        {
            if (nurse.guiMode == WordBreakGUIMode.OnSelected)
                ThaiTextNurse.VisualizeInEditor(nurse);
        }
    }
}