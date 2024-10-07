# Thai Text Care for Unity
This library provides enhanced Thai language support for Unity's TextMeshPro such as Thai Word Segmentation and Thai Font Glyphs fixer for overlapped vowels/tone marks, significantly improving your experience when working with Thai language in Unity.

‎️‍🔥**See it in Action:** [**Try WebGL Demo**](https://phanphantz.github.io/ThaiTextCare-Demo/)‎️‍🔥 (Best Viewed on PC)<br>
☕**Buy Me a Coffee:** If this project made your day a bit easier, consider [**Buying me a coffee**](https://buymeacoffee.com/phanphantz)☕

**Tested On:**
- ✅ **`Unity Editor 2021.3.2f`** with **`TextMeshPro 3.0.6`**
- ✅ **`Unity Editor 2022.3.22f`** with **`TextMeshPro 3.0.9`**
- ❌ Does not support TextMeshPro preview packages

### Overview
- [**ThaiTextNurse**](https://github.com/phanphantz/ThaiTextCare-for-Unity/blob/main/README.md#thaitextnurse) - A Real-time Text Tokenizer component. Provide robust Thai word segmentation on TextMeshPro components.
  - [**Under The Hood**](https://github.com/phanphantz/ThaiTextCare-for-Unity/blob/main/README.md#under-the-hood)
  - [**Handling the Dictionary**](https://github.com/phanphantz/ThaiTextCare-for-Unity/blob/main/README.md#handling-the-dictionary)
  - [**Scripting**](https://github.com/phanphantz/ThaiTextCare-for-Unity/blob/main/README.md#scripting)
- [**ThaiFontDoctor**](https://github.com/phanphantz/ThaiTextCare-for-Unity/blob/main/README.md#thaifontdoctor) - An Editor tool for automating Thai Font Glyph Adjustments for TMP_FontAsset. Can be used to solve overlapped vowels/tone marks which is a common issue in Thai font rendering.
  - [**Glyph Presets**](https://github.com/phanphantz/ThaiTextCare-for-Unity/blob/main/README.md#glyph-presets)
  - [**Limitations**](https://github.com/phanphantz/ThaiTextCare-for-Unity/blob/main/README.md#limitations)
- **Other Topics:**
  - [**How to Install Thai Text Care**](https://github.com/phanphantz/ThaiTextCare-for-Unity?tab=readme-ov-file#how-to-install-thai-text-care)
  - [**Known Issues**](https://github.com/phanphantz/ThaiTextCare-for-Unity?tab=readme-ov-file#known-issues)
  - [**Future Plans**](https://github.com/phanphantz/ThaiTextCare-for-Unity?tab=future-plans)
  - [**Thai Font Modification using FontForge**](https://github.com/phanphantz/ThaiTextCare-for-Unity?tab=readme-ov-file#thai-font-modification-using-fontforge)
  - [**Credits & Inspirations**](https://github.com/phanphantz/ThaiTextCare-for-Unity?tab=readme-ov-file#credits--inspirations)
 
<br>
<p align="center">
<img src="https://github.com/phanphantz/GameDevSecretSauce/blob/main/Assets/ThaiFontDoctor/ThaiTextCare_GIF.gif" width="65%">  
</p>
<p align="center">
<i>Thai Word Segmentation</i>
</p>
<br>
<p align="center">
<img src="https://github.com/phanphantz/GameDevSecretSauce/blob/main/Assets/ThaiFontDoctor/ThaiFontDoctor_GIF.gif" width="65%">
</p>
<p align="center">
<i>Thai Glyph Adjustment Automation</i>
</p>
<br>

# ThaiTextNurse
This component tokenizes and separates Thai words on TextMeshPro components using **`Zero Width Space`**. Just attach it to any TextMeshPro component and you're all set! It will beautifully wrap the Thai text for you!

<img src="https://github.com/phanphantz/GameDevSecretSauce/blob/main/Assets/ThaiFontDoctor/ThaiTextNurse.jpeg" width="1000">

**Key Features**
- **Real-time Tokenization** - It updates in real-time when the text is modified via the Inspector or Script.
- **Word Breaks GUI** - The word segmentation is visualized in the **Scene View**. You can configure the visualization using **`GUIMode`** and **`GUIColor`**
- **Dictionary Edit** - If tokenization is incorrect for unknown words, you can easily Add or Remove words from the dictionary directly through the **Dictionary** section on the ThaiTextNurse Inspector.
- **Custom Separator** - You can insert a **custom separator** before each word breaks.
- **Glyph Correction** - You can set **`Correction`** option to **YoorYingAndToorTaan or FullC90** to fix the issues where the lower vowels like `'ุ'` and `'ู'` get overlapped with characters like `'ญ'` and `'ฐ'`. But the font you use must support [**C90 encoding**](https://github.com/SaladLab/Unity3D.ThaiFontAdjuster/blob/master/docs/UnderTheHood.md#the-c90-encoding-for-thai). If not, you can try to modify it using [**Fontforge**](https://fontforge.org/en-US/). See [**Thai Font Modification using FontForge**](https://github.com/phanphantz/ThaiTextCare-for-Unity?tab=readme-ov-file#thai-font-modification-using-fontforge) for more details.

## Under The Hood
- ThaiTextNurse makes no changes to the original text instead, it only modifies the displayed message using the `TMP_Text` component's **`ITextPreprocessor`** feature, so you can add or remove it without any side effects.
- Behind the scenes, it uses a TrieNode structure with the Longest Match Searching technique. See [`PhTokenizer.cs`](./Runtime/ThaiTextNurse/PhTokenizer.cs) for searching and tokenization logic.
- The word segmentation relies on a simple **Text Dictionary** file located under the **Resources** folder.

## Handling the Dictionary
- The dictionary used by the ThaiTextCore package must always be under any of the project folders named **'Resources'**. You can specify the resource sub-directory path and other global settings of ThaiTextCare at **Project Settings > Thai Text Care Settings**
- The dictionary is loaded/reloaded automatically when :
  - UnityEditor starts up.
  - Scripts of ThaiTextCare assembly recompile.
  - The first ThaiTextNurse triggers the first tokenization during runtime.
  - You **Force Rebuild** the dictionary either by pressing a button on ThaiTextNurse Inspector or by calling **`ThaiTextNurse.RebuildDictionary()`**
- If you modify the dictionary in any way other than Adding or Removing words via the ThaiTextNurse Inspector (e.g., by editing the dictionary file directly), you must manually Force Rebuild the dictionary by yourself.
- In the dictionary file, each word is separated by **new line characters** (`\r\n`, `\n`, `\r`), and words beginning with the **`/`** character are ignored by the tokenizer.

## Scripting

### Runtime & Editor
- You can manually call **`ThaiTextNurse.RebuildDictionary()`** or **`RebuildDictionaryAsync()`** (as a coroutine) to load the dictionary in advance during a loading screen to hide the performance hiccup from dictionary initialization which can be noticeable on low-end devices.
- To get a tokenized version of any string, use one of the **`ThaiTextNurse.SafeTokenize()`** or **`TryTokenize()`** methods

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
- In Editor, you can utilize **`ThaiTextCareEditorCore.AddWordsToDictionary()`**, **`RemoveWordsFromDictionary()`**, or **`SaveDictionaryAsset()`** to modify the dictionary via Editor scripting.

# ThaiFontDoctor
ThaiFontDoctor is a ScriptableObject that processes TextMeshPro's TMP_FontAsset, automating adjustments to glyph pairs based on predefined Glyph Combinations.

When you set a Glyph Combination, you specify which Thai character glyphs should pair together and the appropriate offset for each pair. ThaiFontDoctor then updates the GlyphAdjustmentTable in your TMP_FontAsset in real-time, making it easy to fine-tune how vowels and tone marks appear in your TMP_Text components.

Take a look at **`ThaiFontDoctor.asset`** for an example. This instance of ThaiFontDoctor's ScriptableObject already has some common GlyphCombinations for solving Thai font issues on it:

<img src="https://github.com/phanphantz/GameDevSecretSauce/blob/main/Assets/ThaiFontDoctor/ThaiFontDoctor_Example.png" width="1000">

**Key Features**
- Use the **Add** button to create new Glyph Combinations or click on existing ones to edit them.
- On each GlyphCombination, you can specify the characters and adjustment offset of the **Leading Glyphs** and **Following Glyphs**. 
- Use the **Create In Scene** button behind the **Tester TMP Text** field to quickly create a TMP Text component to see how the font is displayed in real-time.
- Every action performed on the **ThaiFontDoctor** inspector is fully Undo-supported, so you can safely experiment with your adjustments.
- When modifying Glyph Combination, You can use **`GlyphPreset`** to easily insert common Thai character sets, or choose the **กำหนดเอง (Custom)** preset to fine-tune specific glyphs as needed.
  
## Glyph Presets
Here are the available **`GlyphPresets`** and its glyph members :
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

## Limitations
- If multiple Glyph Combinations target the same glyphs, only the last adjustment will be applied. You can reorder combinations by selecting one of them and using the Up Arrow or Down Arrow buttons to prioritize adjustments.
- The multi-edit feature is not supported.

# How to Install Thai Text Care
You have 2 options for installing the library. Either via the package manager (Recommended) or by downloading this repository and putting it in your Unity Projects. 

### Package Manager Installation
Installing the package via the Package Manager allows you to easily install or update ThaiTextCare as a third-party library. However, this method is not suitable if you wish to modify the source code.
1. In UnityEditor, Go to **Window > Package Manager**
2. Click + button and choose **Add package by git URL**
3. Use this link to install the package: https://github.com/phanphantz/ThaiTextCare-for-Unity.git
4. That's it! You're all set!

**TIP:** You can drag the **Examples** folder under **Packages/Thai Text Care/** into the **Assets** folder to easily copy example assets into your project.
<img src="https://github.com/phanphantz/GameDevSecretSauce/blob/main/Assets/ThaiFontDoctor/ThaiTextCare_ExamplesFolder.png" width="300">

# Known Issues
- If your font asset modifications aren't showing up in TextMeshPro components, it might indicate an issue with TextMeshPro's `EventManager` logic. If this happens, try restarting the Unity Editor to resolve the problem.
- You may encounter warnings like 'Unable to add the requested character to font asset ....'s atlas texture. Please make the texture .... readable' To fix this, follow the instructions on [this link](https://discussions.unity.com/t/unable-to-add-character-to-font-assets-atlas-texture/900612) to mark the font texture as readable.
- While ThaiTextNurse works with TextMeshPro components in `TMP_InputField`, the caret position in the input field may display incorrectly due to the injected Zero Width Space characters in the text. I have yet to find an effective solution to fully support `TMP_InputField`.

# Future Plans
- Support the preview versions of the TextMeshPro package
- Support using **`Addressables`** to load and manage the dictionary instead of **`Resources`**
- Runtime modifications of the dictionary

# Thai Font Modification using FontForge
To modify your Thai font extended character glyphs to have the correct Unicode, follow these steps :
1. Download [**FontForge**](https://fontforge.org/en-US/)
2. Open the desired font with **FontForge** (On Windows, you can use **Open With...** from the Right-click menu and Choose **FontForge**)
3. You will need to map the extended characters to the correct Unicode by yourself by right-clicking on the target glyph **> Glyph Info...** > Then edit the **Unicode** field to the correct value.

A Python script can also be used to automate the process. You can copy glyphs by their names to the target Unicode, but the challenge is that many fonts use different naming conventions for their characters.

According to [**C90 encoding**](https://github.com/SaladLab/Unity3D.ThaiFontAdjuster/blob/master/docs/UnderTheHood.md#the-c90-encoding-for-thai), Extended glyphs consist of:

| Code      | Description         | C90 Category  |
| --------- | ------------------- | ------------- |
| U+F700    | uni0E10.descless    | base.descless |
| U+F701~04 | uni0E34~37.left     | upper.left    |
| U+F705~09 | uni0E48~4C.lowleft  | top.lowleft   |
| U+F70A~0E | uni0E48~4C.low      | top.low       |
| U+F70F    | uni0E0D.descless    | base.descless |
| U+F710~12 | uni0E31,4D,47.left  | upper.left    |
| U+F713~17 | uni0E48~4C.left     | top.left      |
| U+F718~1A | uni0E38~3A.low      | lower.low     |

Here’s a summary of the Thai characters in each row:
- U+F700: ฏ (To Patak)
- U+F701~04: ิ, ี, ึ, ื (Sara I, Sara II, Sara UE, Sara UEE)
- U+F705~09: ่, ้, ๊, ๋, ์ (Tone marks: Mai Ek, Mai Tho, Mai Tri, Mai Chattawa, Thanthakhat)
- U+F70A~0E: ่, ้, ๊, ๋, ์ (Same tone marks as above, different positioning)
- U+F70F: ญ (Yo Ying)
- U+F710~12: ั, ํ, ็ (Sara Am, Yamakkan, Mai Taikhu)
- U+F713~17: ่, ้, ๊, ๋, ์ (Tone marks again, different positioning)
- U+F718~1A: ุ, ู, ฺ (Sara U, Sara UU, Phinthu)

4. After you modify the glyphs, You can now Export the font and use it to create TMP_FontAsset in Unity.

# Credits & Inspirations
- Huge thanks to **SaladLab** for the [**ThaiFontAdjuster**](https://github.com/SaladLab/Unity3D.ThaiFontAdjuster), which is the approach for correcting characters using extended glyphs.
- I would also like to express my gratitude to **Chaiwat Matarak** for [**ThaiStringTokenizer**](https://github.com/chaiwatmat/ThaiStringTokenizer). The dictionary file used in this project is based on his work, and it also inspired the creation of my own PhTokenizer class.
- A special thanks to **Onchulee** for the [**ThaiText**](https://github.com/Onchulee/ThaiText) repository which inspired me about how to handle the Dictionary effectively. His C# implementation of the **LexTo** library for word segmentation is also an impressive work.
- And of course, Big thanks to all of you amazing supporters!

You are all true heroes🙏

With gratitude,
<br>Phun
