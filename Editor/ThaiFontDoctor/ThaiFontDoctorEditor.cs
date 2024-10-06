using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using TMP_Text = TMPro.TMP_Text;

namespace PhEngine.ThaiTextCare.Editor
{
    [CustomEditor(typeof(ThaiFontDoctor))]
    public class ThaiFontDoctorEditor : UnityEditor.Editor
    {
        ThaiFontDoctor thaiFontDoctor;
        string targetCharacters = "";
        string pairCharacters = "";
        GUIStyle glyphPreviewStyle;
        GUIStyle combinationHeaderStyle;
        GUIStyle largeLabelStyle;
        int selectedIndex = -1;
        Color orangeColor = new Color(0.8f, 0.5f, 0);
        
        void OnEnable()
        {
            Undo.undoRedoPerformed -= ApplyChanges;
            Undo.undoRedoPerformed += ApplyChanges;
            thaiFontDoctor = target as ThaiFontDoctor;
        }

        void OnDisable()
        {
            Undo.undoRedoPerformed -= ApplyChanges;
        }

        public override void OnInspectorGUI()
        {
            SetupStyles();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Thai Font Doctor", largeLabelStyle, GUILayout.Height(40f));
            ThaiTextCareGUI.DrawBugReportButton();
         
            EditorGUILayout.EndHorizontal();
            ThaiTextCareGUI.DrawHorizontalLine();
            EditorGUILayout.Space();

            var combinationList = thaiFontDoctor.glyphCombinationList;
            DrawFontAssetSection(combinationList);
            if (thaiFontDoctor.fontAsset == null)
            {
                EditorGUILayout.HelpBox("Please assign a font asset file", MessageType.Info);
                return;
            }
            
            DrawTesterSection();
            EditorGUILayout.Space();
            DrawCombinationAdder(combinationList);
            DrawCombinations(combinationList);
        }

        void SetupStyles()
        {
            glyphPreviewStyle = new GUIStyle(EditorStyles.label);
            glyphPreviewStyle.fontSize = 20;
            glyphPreviewStyle.normal.textColor = orangeColor;
            
            combinationHeaderStyle = new GUIStyle(EditorStyles.boldLabel);
            combinationHeaderStyle.fontSize = 15;

            largeLabelStyle = new GUIStyle(EditorStyles.boldLabel);
            largeLabelStyle.fontSize = 30;
        }

        void DrawFontAssetSection(List<GlyphCombination> combinationList)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            var fontAsset = EditorGUILayout.ObjectField(new GUIContent("Font Asset"), thaiFontDoctor.fontAsset, typeof(TMP_FontAsset), false) as TMP_FontAsset;
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(thaiFontDoctor, "Change font asset");
                thaiFontDoctor.SetFontAsset(fontAsset);
                ApplyChanges();
            }
            if (thaiFontDoctor.fontAsset)
            {
                if (GUILayout.Button("Clean & Rebuild", GUILayout.ExpandWidth(false)))
                {
                    var result = EditorUtility.DisplayDialog($"Confirmation", $"Are you sure you want to Clean and Rebuild all the glyph pair adjustments?\n\nAny pair adjustment you made directly to the font asset will be lost", "Yes", "No");
                    if (result)
                    {
                        Undo.RecordObject(thaiFontDoctor.fontAsset, "Perform Clean Rebuild");
                        thaiFontDoctor.CleanAndRebuild();
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            if (fontAsset && combinationList.Count > 0)
            {
                EditorGUI.indentLevel++;
                var variantCount = combinationList.Sum(i => i.VariantCount);
                EditorGUILayout.LabelField("Combination Variant Count : " + variantCount);
                EditorGUILayout.LabelField("Actual Pair Adjustment Count : " + fontAsset.fontFeatureTable.glyphPairAdjustmentRecords.Count);
                EditorGUI.indentLevel--;
            }
        }

        void DrawTesterSection()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            var testerTMPText = EditorGUILayout.ObjectField("Tester TMP Text", thaiFontDoctor.TesterTMPText, typeof(TMP_Text), true) as TMP_Text;
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(thaiFontDoctor, "Change Tester TMP Text");
                thaiFontDoctor.SetTesterTMPText(testerTMPText);
            }

            if (testerTMPText == null)
            {
                if (GUILayout.Button("Create In Scene", GUILayout.ExpandWidth(false)))
                    CreateTesterTMPText();
            }
            else
            {
                if (GUILayout.Button("Dispose", GUILayout.ExpandWidth(false)))
                    DestroyImmediate(testerTMPText.gameObject);
            }
            EditorGUILayout.EndHorizontal();
            
            //Test text
            EditorGUI.BeginChangeCheck();
            var testText = EditorGUILayout.TextArea(thaiFontDoctor.testMessage);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(thaiFontDoctor, "Change Test Text");
                thaiFontDoctor.SetTestText(testText);
            }
        }
        
        void DrawCombinationAdder(List<GlyphCombination> combinationList)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Glyph  Combination List (" + combinationList.Count + ")", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add", GUILayout.ExpandWidth(false)))
            {
                Undo.RecordObject(thaiFontDoctor, "Add new combination");
                combinationList.Add(new GlyphCombination());
                ApplyChanges();
            }
            EditorGUILayout.EndHorizontal();
            ThaiTextCareGUI.DrawHorizontalLine();
        }

        void CreateTesterTMPText()
        {
            GameObject textObject = new GameObject("TMP_Text_FontTester");
            var textMeshPro = textObject.AddComponent<TextMeshPro>();
            textMeshPro.rectTransform.sizeDelta = thaiFontDoctor.testerTMPSize;
            textMeshPro.fontSize = 24;
            textMeshPro.alignment = TextAlignmentOptions.Center;
            textMeshPro.color = Color.white;
            thaiFontDoctor.SetTesterTMPText(textMeshPro);
            Undo.RegisterCreatedObjectUndo(textObject, "Create TextMeshPro Object");
            Selection.activeGameObject = textObject;
            EditorGUIUtility.PingObject(textObject);
            SceneView.lastActiveSceneView.FrameSelected();
            Selection.activeGameObject = null;
            Selection.SetActiveObjectWithContext(thaiFontDoctor, thaiFontDoctor);
        }

        void DrawCombinations(List<GlyphCombination> combinationList)
        {
            var i = 0;
            foreach (var combination in combinationList)
            {
                if (DrawCombination(combinationList, i, combination)) 
                    return;
                
                i++;
            }
        }

        bool DrawCombination(List<GlyphCombination> combinationList, int i, GlyphCombination combination)
        {
            var elementStartRegion = GUILayoutUtility.GetRect(0f, 0f, GUILayout.ExpandWidth(true));
            if (DrawCombinationHeader(combinationList, i, combination)) 
                return true;
            
            if (selectedIndex == i)
            {
                EditorGUI.BeginDisabledGroup(!combination.isEnabled);
                ThaiTextCareGUI.DrawHorizontalLine();
                DrawCharacterAdder("Leading Glyphs ("+ combination.first.glyphs.Length + ")", ref targetCharacters, ref combination.first);
                DrawCharacterAdder("Following Glyphs (" + combination.second.glyphs.Length + ")",ref pairCharacters, ref combination.second);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(combination.VariantCount + " Total Pairs", EditorStyles.miniLabel,GUILayout.ExpandWidth(false));
                GUILayout.FlexibleSpace();
                EditorGUI.EndDisabledGroup();
                if (GUILayout.Button("Duplicate", EditorStyles.linkLabel, GUILayout.ExpandWidth(false)))
                {
                    Undo.RecordObject(thaiFontDoctor, "Remove combination");
                    var targetIndex = combinationList.IndexOf(combination) + 1;
                    combinationList.Insert(targetIndex,new GlyphCombination(combination));
                    selectedIndex = targetIndex;
                    return true;
                }
            }
            EditorGUILayout.EndVertical();
            
            //Draw selction outline
            if (selectedIndex == i)
            {
                EditorGUILayout.EndHorizontal();
                var elementEndRegion = GUILayoutUtility.GetRect(0f, 0f, GUILayout.ExpandWidth(true));
                var selectionArea = new Rect(elementStartRegion.x, elementStartRegion.y, elementEndRegion.width, elementEndRegion.y - elementStartRegion.y);
                DrawBox(selectionArea, 1f, orangeColor);
            }

            return false;
        }

        static void DrawBox(Rect rect, float thickness, Color color)
        {
            EditorGUI.DrawRect(new Rect(rect.x - thickness, rect.y + thickness, rect.width + thickness * 2, thickness), color);
            EditorGUI.DrawRect(new Rect(rect.x - thickness, rect.y + thickness, thickness, rect.height - thickness * 2), color);
            EditorGUI.DrawRect(new Rect(rect.x - thickness, rect.y + rect.height - thickness * 2, rect.width + thickness * 2, thickness), color);
            EditorGUI.DrawRect(new Rect(rect.x + rect.width, rect.y + thickness, thickness, rect.height - thickness * 2), color);
        }


        bool DrawCombinationHeader(List<GlyphCombination> list, int i, GlyphCombination combo)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            
            EditorGUI.BeginChangeCheck();
            var isEnabled = EditorGUILayout.Toggle(combo.isEnabled, GUILayout.Width(15));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(thaiFontDoctor, "Toggle Combination");
                combo.isEnabled = isEnabled;
                ApplyChanges();
            }

            var entryTitle = (i + 1) + ". " + combo.DisplayName;
            if (GUILayout.Button(entryTitle, combinationHeaderStyle))
                ToggleSelection(i);
            
            if (i == selectedIndex && DrawReorderButtons(list, combo))
                return true;

            if (GUILayout.Button("x", EditorStyles.boldLabel, GUILayout.ExpandWidth(false)))
            {
                Undo.RecordObject(thaiFontDoctor, "Remove combination");
                list.Remove(combo);
                ApplyChanges();
                return true;
            }
            EditorGUILayout.EndHorizontal();
            return false;
        }

        void ToggleSelection(int i)
        {
            if (selectedIndex == i)
                selectedIndex = -1;
            else
                selectedIndex = i;
        }

        bool DrawReorderButtons(List<GlyphCombination> list, GlyphCombination combo)
        {
            if (GUILayout.Button("\u2193", GUILayout.ExpandWidth(false)))
            {
                Undo.RecordObject(thaiFontDoctor, "Move combination down");
                var targetIndex = Mathf.Min(list.IndexOf(combo) + 1, list.Count - 1);
                list.Remove(combo);
                list.Insert(targetIndex, combo);
                selectedIndex = targetIndex;
                ApplyChanges();
                return true;
            }

            if (GUILayout.Button("\u2191", GUILayout.ExpandWidth(false)))
            {
                Undo.RecordObject(thaiFontDoctor, "Move combination up");
                var targetIndex = Mathf.Max(list.IndexOf(combo) - 1, 0);
                list.Remove(combo);
                list.Insert(targetIndex, combo);
                selectedIndex = targetIndex;
                ApplyChanges();
                return true;
            }

            return false;
        }

        void DrawCharacterAdder(string fieldName, ref string newCharacters, ref GlyphGroup group)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(fieldName, EditorStyles.boldLabel);
            
            EditorGUI.indentLevel++;
            if (!string.IsNullOrEmpty(group.glyphs.Trim()))
                DrawPlacementAdjustor(group);

            EditorGUI.BeginChangeCheck();
            var values = (ThaiGlyphPreset[])Enum.GetValues(typeof(ThaiGlyphPreset));
            var popups = values.Select(GetDisplayedPresetString).ToArray();
            var currentIndex = Array.IndexOf(popups, GetDisplayedPresetString(group.preset));
            var newIndex = EditorGUILayout.Popup("Glyph Preset", currentIndex, popups);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(thaiFontDoctor, "Change Glyph Preset");
                group.AssignGroup((ThaiGlyphPreset)newIndex);
                ApplyChanges();
            }
            
            if (group.preset == ThaiGlyphPreset.Custom)
                TryDrawGlyphAdder(ref newCharacters, group);
            EditorGUI.indentLevel--;

            TryDrawGlyphPreviews(group.preset == ThaiGlyphPreset.Custom, ref group.glyphs);
            EditorGUILayout.EndVertical();
        }

        string GetDisplayedPresetString(ThaiGlyphPreset preset)
        {
            return $"{ThaiGlyphHelper.GetThaiGlyphGroupName(preset)} ({preset})";
        }

        void TryDrawGlyphAdder(ref string newCharacters, GlyphGroup group)
        {
            newCharacters = EditorGUILayout.TextField("Add Glyph: " , newCharacters);
            var isInputEmpty = string.IsNullOrEmpty(newCharacters.Trim());
            if (isInputEmpty)
                return;
            
            EditorGUILayout.BeginHorizontal();
            var displayedGlyphs = newCharacters.Select(c => ThaiGlyphHelper.GetDisplayedString(c.ToString()));
            EditorGUILayout.LabelField("Preview : ", EditorStyles.boldLabel, GUILayout.Width(95));
            EditorGUILayout.LabelField(string.Join(", ", displayedGlyphs), glyphPreviewStyle);
            if (GUILayout.Button("Add", GUILayout.ExpandWidth(false)))
            {
                Undo.RecordObject(thaiFontDoctor, "Modify Characters");
                group.glyphs = string.Join("",group.glyphs.ToCharArray().Union(newCharacters.ToCharArray()));
                newCharacters = "";
                ApplyChanges();
            }
            EditorGUILayout.EndHorizontal();
        }

        void DrawPlacementAdjustor(GlyphGroup group)
        {
            EditorGUI.BeginChangeCheck();
            var offsetVector = EditorGUILayout.Vector2Field("Offset", new Vector2(group.xPlacement, group.yPlacement));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(thaiFontDoctor, "Change Target Character");
                group.xPlacement = offsetVector.x;
                group.yPlacement = offsetVector.y;
                ApplyChanges();
            }
        }

        void TryDrawGlyphPreviews(bool isAllowDeleting, ref string characters)
        {
            if (string.IsNullOrEmpty(characters))
                return;

            var cellSize = 25f;
            var cellPerRow = Mathf.FloorToInt(EditorGUIUtility.currentViewWidth / (cellSize * 2.5f));
            EditorGUILayout.BeginVertical();
            var isSomeGlyphInvalid = false;
            for (int i = 0; i < characters.Length; i++)
            {
                if (i == 0)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("", GUILayout.Width(10));
                }

                if (!DrawGlyphPreview(isAllowDeleting, ref characters, cellSize, characters[i]))
                    isSomeGlyphInvalid = true;
                
                if ((i + 1) % cellPerRow == 0)
                {
                    EditorGUILayout.EndHorizontal();
                    if (i == characters.Length - 1) 
                        continue;
                    
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("", GUILayout.Width(10));
                }
                else if (i == characters.Length - 1)
                {
                    EditorGUILayout.EndHorizontal();

                }
            }
            EditorGUILayout.EndVertical();
            if (isSomeGlyphInvalid)
                EditorGUILayout.HelpBox("Some glyphs are missing from the font asset", MessageType.Error);
        }

        bool DrawGlyphPreview(bool isAllowDeleting, ref string characters, float cellSize, char character)
        {
            var oldColor = GUI.color;
            var isValid = true;
            if (!ThaiFontDoctor.TryGetGlyphIndex(thaiFontDoctor.fontAsset, character, out _, out _))
            {
                GUI.color = Color.red;
                isValid = false;
            }
            
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.Width(cellSize));
            var displayedString = ThaiGlyphHelper.GetDisplayedString(character.ToString());
            EditorGUILayout.LabelField(" "+ displayedString, glyphPreviewStyle, GUILayout.Width(cellSize), GUILayout.Height(25f));
            GUI.color = oldColor;
            
            if (isAllowDeleting)
            {
                if (GUILayout.Button("x", EditorStyles.boldLabel ,GUILayout.ExpandWidth(false)))
                {
                    Undo.RecordObject(thaiFontDoctor, "Modify Characters");
                    characters = characters.Replace(character.ToString(), "");
                    ApplyChanges();
                }
            }
            EditorGUILayout.EndHorizontal();
            return isValid;
        }
        
        void ApplyChanges()
        {
            thaiFontDoctor.ApplyModifications();
        }
    }
}