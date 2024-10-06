#if UNITY_EDITOR
using UnityEditor;
#endif

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
        public const string SettingsPath = "Assets/Plugins/ThaiTextCare/Resources/ThaiTextCareSettings.asset";
        
        public static ThaiTextCareSettings PrepareInstance()
        {
            if (unsafeInstance == null)
            {
                unsafeInstance = Resources.Load<ScriptableObject>("ThaiTextCareSettings") as ThaiTextCareSettings;
                if (unsafeInstance == null)
                {
#if UNITY_EDITOR
                    var path = SettingsPath;
                    unsafeInstance = CreateInstance<ThaiTextCareSettings>();
                    var directory = Path.GetDirectoryName(path);
                    if (!Directory.Exists(directory) && directory != null)
                            Directory.CreateDirectory(directory);

                    AssetDatabase.CreateAsset(unsafeInstance, path);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Created a default ThaiTextNurseSettings at : " + path);
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