# Thai Text Care for Unity
Library for Enhanced Thai Language Support in Unity's TextMeshPro

# Features
- [**ThaiTextNurse**]() - A Real-time text Tokenizer component for Thai word segmentation on TextMeshPro components.
- [**ThaiFontDoctor**]() - An Editor tool for automating Thai Font Glyph Adjustments for TMP_FontAsset.

## ThaiTextNurse
This component is a non-invasive solution for tokenizing and separating Thai words on TextMeshPro components. Just attach it to any TextMeshPro with Thai text, and you're all set! It will beautifully wrap the Thai text for you.

<img>

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

## Using ThaiTextCare APIs

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

# Thai Font Modification using FontForge
