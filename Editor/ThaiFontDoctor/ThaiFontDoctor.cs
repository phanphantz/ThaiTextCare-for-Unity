using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

#if TMP_3_2_0_PRE_2 || UGUI_2_0

using GlyphAdjustmentRecord = UnityEngine.TextCore.LowLevel.GlyphAdjustmentRecord;
using GlyphPairAdjustmentRecord = UnityEngine.TextCore.LowLevel.GlyphPairAdjustmentRecord;
using GlyphValueRecord = UnityEngine.TextCore.LowLevel.GlyphValueRecord;

#else

using GlyphAdjustmentRecord = TMPro.TMP_GlyphAdjustmentRecord;
using GlyphPairAdjustmentRecord = TMPro.TMP_GlyphPairAdjustmentRecord;
using GlyphValueRecord = TMPro.TMP_GlyphValueRecord;

#endif

namespace PhEngine.ThaiTextCare.Editor
{
    [CreateAssetMenu(menuName = "ThaiTextCare/ThaiFontDoctor", fileName = "ThaiFontDoctor", order = 0)]
    public class ThaiFontDoctor : ScriptableObject
    {
        public TMP_Text TesterTMPText => testerTMPText;
        [SerializeField] TMP_Text testerTMPText;
        
        public Vector2 testerTMPSize = new Vector2(100f, 100f);
        public string testMessage = @"ปิ่น อื้อ จี๊ด อื้ม ปรื๋อ ผื่น ลิ้น 
ติ๋ม ปริ่ม หั่น ปั้น ตั๊ก ป้า ม๊า 
ฝ่า ป่า ฟ้า ผ่า ผ้า จ๋า สิทธิ์  
ย่ำ ถ้ำ ฎุ ฎูำ";

        public TMP_FontAsset fontAsset;
        public List<GlyphCombination> glyphCombinationList = new List<GlyphCombination>();

        [HideInInspector, SerializeField] List<GlyphPairAdjustmentRecord> cachedPairList = new List<GlyphPairAdjustmentRecord>();

        [ContextMenu(nameof(ApplyModifications))]
        public void ApplyModifications()
        {
            if (fontAsset == null)
                return;

            Undo.RecordObject(fontAsset, "Modify Pair Adjustments");
            DisposeCachedPairs();
            var enabledCombinations = glyphCombinationList.Where(c => c.isEnabled).ToArray();
            foreach (var combination in enabledCombinations)
                InjectCombination(combination);
            ApplyChangesToFontAsset();
            ApplyToTesterText();
        }

        void DisposeCachedPairs()
        {
            foreach (var pairRecord in cachedPairList)
                fontAsset.fontFeatureTable.glyphPairAdjustmentRecords.RemoveAll(r =>
                    r.firstAdjustmentRecord.glyphIndex == pairRecord.firstAdjustmentRecord.glyphIndex &&
                    r.secondAdjustmentRecord.glyphIndex == pairRecord.secondAdjustmentRecord.glyphIndex);

            cachedPairList.Clear();
        }
        
        void InjectCombination(GlyphCombination combination)
        {
            var targetCharacters = combination.first.glyphs.Trim();
            var pairCharacters = combination.second.glyphs.Trim();
            foreach (var targetCharacter in targetCharacters)
            {
                if (!TryGetGlyphIndex(fontAsset, targetCharacter, out var targetGlyphIndex, out _))
                    continue;

                foreach (var pairCharacter in pairCharacters)
                {
                    if (!TryGetGlyphIndex(fontAsset, pairCharacter, out var pairGlyphIndex, out _))
                        continue;

                    var targetPair = GetPairAdjustmentRecord(targetGlyphIndex, pairGlyphIndex);
                    ModifyPair(targetPair, combination);
                }
            }
        }
        
        GlyphPairAdjustmentRecord GetPairAdjustmentRecord(uint firstGlyphIndex, uint secondGlyphIndex)
        {
            var adjustmentRecords = fontAsset.fontFeatureTable.glyphPairAdjustmentRecords;
            foreach (var record in adjustmentRecords)
            {
                if (record.firstAdjustmentRecord.glyphIndex == firstGlyphIndex &&
                    record.secondAdjustmentRecord.glyphIndex == secondGlyphIndex)
                    return record;
            }
            
            var firstAdjustment = new GlyphAdjustmentRecord(firstGlyphIndex, new GlyphValueRecord());
            var secondAdjustment = new GlyphAdjustmentRecord(secondGlyphIndex, new GlyphValueRecord());
            var newPairRecord = new GlyphPairAdjustmentRecord(firstAdjustment, secondAdjustment);
            fontAsset.fontFeatureTable.glyphPairAdjustmentRecords.Add(newPairRecord);
            return newPairRecord;
        }
        
        void ModifyPair(GlyphPairAdjustmentRecord pairRecord, GlyphCombination glyphCombination)
        {
            var firstRecord = pairRecord.firstAdjustmentRecord;
            firstRecord.glyphValueRecord = ApplyPlacement(glyphCombination.first, firstRecord);

            var secondRecord = pairRecord.secondAdjustmentRecord;
            secondRecord.glyphValueRecord = ApplyPlacement(glyphCombination.second, secondRecord);

            var modifiedPair = new GlyphPairAdjustmentRecord(firstRecord, secondRecord);
            fontAsset.fontFeatureTable.glyphPairAdjustmentRecords.Remove(pairRecord);
            fontAsset.fontFeatureTable.glyphPairAdjustmentRecords.Add(modifiedPair);
            cachedPairList.Add(modifiedPair);
        }

        void ApplyChangesToFontAsset()
        {
            fontAsset.ReadFontAssetDefinition();
            TMPro_EventManager.ON_FONT_PROPERTY_CHANGED(true, fontAsset);
            EditorUtility.SetDirty(fontAsset);
        }

        void ApplyToTesterText()
        {
            if (!testerTMPText) 
                return;
            
            testerTMPText.font = fontAsset;
            testerTMPText.text = testMessage;
        }

        public void CleanAndRebuild()
        {
            fontAsset.fontFeatureTable.glyphPairAdjustmentRecords.Clear();
            EditorUtility.SetDirty(fontAsset);
            ApplyModifications();
        }

        static GlyphValueRecord ApplyPlacement(GlyphGroup group, GlyphAdjustmentRecord secondRecord)
        {
            var record = secondRecord.glyphValueRecord;
            record.xPlacement = group.xPlacement;
            record.yPlacement = group.yPlacement;
            return record;
        }

        public static bool TryGetGlyphIndex(TMP_FontAsset fontAsset, char character, out uint glyphIndex,
            out int characterIndex)
        {
            glyphIndex = 0;
            characterIndex = -1;
            var characterData = fontAsset.characterTable.FirstOrDefault(c => c.unicode == character);
            if (characterData != null)
            {
                glyphIndex = characterData.glyphIndex;
                characterIndex = fontAsset.characterTable.IndexOf(characterData);
                return true;
            }

            return false;
        }

        public void SetTesterTMPText(TMP_Text tmpText)
        {
            testerTMPText = tmpText;
            testerTMPText.font = fontAsset;
            ApplyToTesterText();
        }

        public void SetTestText(string value)
        {
            testMessage = value;
            ApplyToTesterText();
        }

        public void SetFontAsset(TMP_FontAsset target)
        {
            fontAsset = target;
            ApplyToTesterText();
        }
    }
}