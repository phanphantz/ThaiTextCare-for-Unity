#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.IO;
using UnityEngine;

namespace PhEngine.ThaiTextCare
{
    public class ThaiTextCareSettings : ScriptableObject
    {
        public string WordBreakCharacter => wordBreakType == WordBreakType.ZeroWidthSpace
            ? "​"
            : customCharacter;
        
        [Header("General")]
        [SerializeField] string dictionaryResourcePath = "dictionary";
        public string DictionaryResourcePath => dictionaryResourcePath;
        
        [SerializeField] WordBreakType wordBreakType;
        [SerializeField] string customCharacter;
        
        [Header("Editor-Only")]
        [SerializeField] bool loadDictionaryOnStart = true;
        public bool IsLoadDictionaryOnEditorStartUp => loadDictionaryOnStart;
        public const string PluginsFolderPath = "Plugins/ThaiTextCare/Resources/";
        public static string SettingsPath => "Assets/" + PluginsFolderPath + "ThaiTextCareSettings.asset";
        
        public static ThaiTextCareSettings PrepareInstance()
        {
            if (unsafeInstance == null)
            {
                unsafeInstance = Resources.Load<ScriptableObject>("ThaiTextCareSettings") as ThaiTextCareSettings;
                if (unsafeInstance == null)
                {
#if UNITY_EDITOR
                    var path = SettingsPath;
                    Debug.Log("Created a default ThaiTextNurseSettings at : " + path);
                    unsafeInstance = CreateInstance<ThaiTextCareSettings>();
                    var directory = Path.GetDirectoryName(path);
                    if (!Directory.Exists(directory) && directory != null)
                        Directory.CreateDirectory(directory);

                    AssetDatabase.CreateAsset(unsafeInstance, path);
                    AssetDatabase.SaveAssets();
                    
                    //Check if asset is added to the project as a package
                    var packageDictionaryPath = Path.GetFullPath("Packages/com.phengine.thaitextcare/Resources/dictionary.txt");
                    if (!string.IsNullOrEmpty(packageDictionaryPath))
                    {
                        if (ThaiTextNurse.TryLoadDictionaryAsset(unsafeInstance, out var defaultTextAsset))
                        {
                            // We already have the dictionary, nothing to do. Unload it
                            Resources.UnloadAsset(defaultTextAsset);
                        }
                        else
                        {
                            Debug.Log("Created a default dictionary at folder: " + PluginsFolderPath);
                            var targetPath = Path.Combine(Application.dataPath, PluginsFolderPath, "dictionary.txt");
                            File.Copy(packageDictionaryPath, targetPath);
                            AssetDatabase.Refresh();
                        }
                    }
#else
                    Debug.LogError("ThaiTextNurseSettings.asset is missing from the Resources folder! the default settings will be used on ThaiTextNurse components");
#endif
                }
            }
            return unsafeInstance;
        }

        static ThaiTextCareSettings unsafeInstance;
    }

    public enum WordBreakType
    {
        ZeroWidthSpace, CustomCharacter
    }
}