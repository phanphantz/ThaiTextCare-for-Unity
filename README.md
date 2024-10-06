# Thai Text Care for Unity
Library for Enhanced Thai Language Support in Unity's TextMeshPro

# Overview
- [**ThaiTextNurse**](https://github.com/phanphantz/ThaiTextCare-for-Unity/blob/main/README.md#thaitextnurse) - A Real-time text Tokenizer component for Thai word segmentation on TextMeshPro components.
- [**ThaiFontDoctor**](https://github.com/phanphantz/ThaiTextCare-for-Unity/blob/main/README.md#) - An Editor tool for automating Thai Font Glyph Adjustments for TMP_FontAsset.

## ThaiTextNurse
This component is a non-invasive solution for tokenizing and separating Thai words on TextMeshPro components. Just attach it to any TextMeshPro with Thai text, and you're all set! It will beautifully wrap the Thai text for you.

<img>

### Key Features
- It updates in real time when the text is modified via the Editor or Script.
- The word segmentation is also visualized in the **Scene View**.
- If tokenization is incorrect for unknown words, you can easily Add or Remove words from the dictionary directly through the **Dictionary** section on the ThaiTextNurse Inspector.
- You can insert a **custom separator** before each word breaks.
- It can help fix the issues where the lower vowels like 'ุ' and 'ู' get overlapped with characters like 'ญ' and 'ฐ'. But the font you use must support [**C90 encoding**](https://github.com/SaladLab/Unity3D.ThaiFontAdjuster/blob/master/docs/UnderTheHood.md#the-c90-encoding-for-thai). If not, you can try to modify it using [**Fontforge**](https://fontforge.org/en-US/). See [**Thai Font Modification using FontForge**]() for more details.

### Under The Hood
- ThaiTextNurse makes no changes to the original text instead, it only modifies the displayed message using the TMP_Text component's **ITextPreprocessor** feature, so you can add or remove it without any side effects.
- Behind the scenes, it uses a TrieNode structure with the Longest Match Searching technique.
- The word segmentation relies on a simple **Text Dictionary** file located under the **Resources** folder.

## Handling the Dictionary
- The dictionary used by the ThaiTextCore package must always be under any of the project folders named **'Resources'**. You can specify the resource sub-directory path and other global settings of ThaiTextCare at **Project Settings > Thai Text Care Settings**
- The dictionary is loaded/reloaded automatically when these events happen :
  - When the UnityEditor starts up.
  - When the scripts of ThaiTextCare assembly recompile.
  - When the first ThaiTextNurse triggers the first tokenization during runtime.
  - When you **Force Rebuild** the dictionary either by pressing a button on ThaiTextNurse Inspector or by calling **ThaiTextNurse.RebuildDictionary()**
- If you modify the dictionary in any way other than Adding or Removing words via the ThaiTextNurse Inspector (e.g., by editing the dictionary file directly), you must manually Force Rebuild the dictionary by yourself.
- In the dictionary file, each word is separated by **new line characters (\r\n, \n, \r)** and words beginning with the **'/'** character are ignored by the tokenizer.

## Using ThaiTextNurse APIs

### Runtime & Editor
- You can manually call **ThaiTextNurse.RebuildDictionary()** or **RebuildDictionaryAsync()** (as a coroutine) to load the dictionary in advance during a loading screen to avoid any lags from initializing the dictionary.
- To get a tokenized version of any string, use one of the **ThaiTextNurse.SafeTokenize()** or **TryTokenize()** methods

```csharp
 // Example input string in Thai
string inputText = "สวัสดีครับ";

// Using SafeTokenize to tokenize the input
 string tokenizedResult = ThaiTextNurse.SafeTokenize(inputText);
 Debug.Log("Tokenized Result: " + tokenizedResult);

// Using TryTokenize for more control
if (ThaiTextNurse.TryTokenize(inputText, out TokenizeResult result))
 {
    Debug.Log("Successfully tokenized!");
    Debug.Log("Tokenized String: " + result.Result);
    Debug.Log("Word Count: " + result.WordCount);
}
else
{
    Debug.LogError("Tokenization failed.");
}
```

### Editor-Only
- In Editor, you can utilize **ThaiTextCareEditorCore.AddWordsToDictionary()**, **RemoveWordsFromDictionary()**, or **SaveDictionaryAsset()** to modify the dictionary via Editor scripting.

## ThaiFontDoctor
ThaiFontDoctor is a ScriptableObject that processes TextMeshPro's TMP_FontAsset, automating adjustments to glyph pairs based on predefined Glyph Combinations.

When you set a Glyph Combination, you specify which Thai character glyphs should pair together and the appropriate offset for each pair. ThaiFontDoctor then updates the GlyphAdjustmentTable in your TMP_FontAsset in real-time, making it easy to fine-tune how vowels and tone marks appear in your TMP_Text components.

Take a look at **ThaiFontDoctor.asset** for an example. This instance of ThaiFontDoctor's ScriptableObject already has some common GlyphCombinations for solving Thai font issues on it:

<img src="../Assets/ThaiFontDoctor/ThaiFontDoctor_Example.png" width="1000">

### Key Features
- Use the **Add** button to create new Glyph Combinations or click on existing ones to edit them.
- On each GlyphCombination, you can specify the characters and adjustment offset of the **Leading Glyphs** and **Following Glyphs**. 
- Use the **Create In Scene** button behind the **Tester TMP Text** field to quickly create a TMP Text component to see how the font is displayed in real-time.
- Every action performed on the **ThaiFontDoctor** inspector is fully Undo-supported, so you can safely experiment with your adjustments.
- When modifying Glyph Combination, You can use **GlyphPreset** to easily insert common Thai character sets, or choose the **กำหนดเอง (Custom)** preset to fine-tune specific glyphs as needed.
  
Here are the available **GlyphPresets** and its glyph members :
| ThaiGlyphPreset | Display Name | Glyph Members | 
|-----------------------|--------------------------|---------------------------------------------------------------------------------------------------| 
| AllConsonants | ก - ฮ | ก, ข, ฃ, ค, ฅ, ฆ, ง, จ, ฉ, ช, ซ, ฌ, ญ, ฎ, ฏ, ฐ, ฑ, ฒ, ณ, ด, ต, ถ, ท, ธ, น, บ, ป, ผ, ฝ, พ, ฟ, ภ, ม, ย, ร, ล, ว, ศ, ษ, ส, ห, ฬ, อ, ฮ | 
| AscenderConsonants | พยัญชนะหางบน | ป, ฝ, ฟ, ฬ | 
| DescenderConsonants | พยัญชนะหางล่าง | ฎ, ฏ | 
| AllUpperGlyphs | อักขระด้านบน |- ิ, - ี, - ึ, - ื, - ็, - ั, - ์, - ่, - ้, - ๊, - ๋ | 
| UpperVowels | สระบน |- ิ, - ี, - ึ, - ื, - ็, - ั | 
| ToneMarks | วรรณยุกต์ |- ่, - ้, - ๊, - ๋ | 
| ThanThaKhaat | ทัณฑฆาต |- ์ | 
| LeadingVowels | สระหน้า | เ-, แ-, โ-, ไ-, ใ- | 
| AllFollowingVowels | สระหลัง | -ะ, - ำ, -า, -ๅ | 
| SaraAum | สระอำ |- ำ | 
| LowerVowels | สระล่าง |- ุ, - ู | 

### Limitations
- If multiple Glyph Combinations target the same glyphs, only the last adjustment will be applied. You can reorder combinations by selecting one of them and use the Up Arrow or Down Arrow buttons to prioritize adjustments.
- ThaiFontDoctor doesn't modify the displayed text based on Unicode replacements. Issues like YoYing ( ญ ) and ThoThan ( ฐ ) incorrectly rendering with lower vowels like Sara Uu ( ู ) remain unresolved for now.
- Multi-edit feature is not supported.

# Thai Font Modification using FontForge
