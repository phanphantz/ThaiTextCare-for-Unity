using UnityEditor;
using UnityEngine.UIElements;

namespace PhEngine.ThaiTextCare.Editor
{
    public class ThaiTextCareSettingsProvider : SettingsProvider
    {
        const string SettingsPath = "Project/Thai Text Care Settings";
        ThaiTextCareSettings settings;
        SerializedObject serializedObject;

        public ThaiTextCareSettingsProvider(string path, SettingsScope scope = SettingsScope.Project) : base(path, scope)
        {
            settings = ThaiTextCareSettings.PrepareInstance();
            serializedObject = new SerializedObject(settings);
        }

        public override void OnGUI(string searchContext)
        {
            ThaiTextCareSettingsEditor.Draw(serializedObject, settings, false);
        }
        
        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            base.OnActivate(searchContext, rootElement);
            Undo.undoRedoPerformed += Repaint;
        }

        public override void OnDeactivate()
        {
            base.OnDeactivate();
            Undo.undoRedoPerformed -= Repaint;
        }

        [SettingsProvider]
        public static SettingsProvider CreateThaiTextNurseSettingsProvider()
        {
            return new ThaiTextCareSettingsProvider(SettingsPath);
        }
    }
}