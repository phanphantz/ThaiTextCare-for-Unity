using System.IO;
using UnityEditor;
using UnityEngine;

namespace PhEngine.ThaiTextCare.Editor
{
    [CustomEditor(typeof(ThaiTextCareSettings))]
    public class ThaiTextCareSettingsEditor : UnityEditor.Editor
    {
        ThaiTextCareSettings settings;

        void OnEnable()
        {
            settings = target as ThaiTextCareSettings;
        }

        public override void OnInspectorGUI()
        {
            Draw(serializedObject, settings, true);
        }

        public static void Draw(SerializedObject serializedObject, ThaiTextCareSettings settings, bool isDrawLocationNotice)
        {
            if (isDrawLocationNotice)
                EditorGUILayout.LabelField("Always keep this asset under: " + Path.GetDirectoryName(ThaiTextCareSettings.SettingsPath), EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            serializedObject.UpdateIfRequiredOrScript();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("dictionaryResourcePath"));
            EditorGUILayout.HelpBox("The path must be under 'Resources' folder. Without file extension.", MessageType.Info);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("wordBreakType"));
            if (serializedObject.FindProperty("wordBreakType").boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("customCharacter"));
                EditorGUILayout.HelpBox("CAUTION: ThaiTextNurse will remove any existing custom characters from text first before tokenize it." , MessageType.Warning);

                EditorGUI.indentLevel--;
            }
            EditorGUILayout.PropertyField(serializedObject.FindProperty("loadDictionaryOnStart"));
            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(settings);
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Apply Changes", GUILayout.ExpandWidth(false)))
            {
                ThaiTextNurse.RebuildDictionary();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}