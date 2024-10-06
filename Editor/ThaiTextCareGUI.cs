using UnityEditor;
using UnityEngine;

namespace PhEngine.ThaiTextCare.Editor
{
    public static class ThaiTextCareGUI
    {
        public static void DrawHorizontalLine()
        {
            var padding = 3f;
            var thickness = 0.75f;
            var r = EditorGUILayout.GetControlRect(GUILayout.Height(padding+thickness));
            r.height = thickness;
            r.y+=padding/2;
            r.x-=2;
            EditorGUI.DrawRect(r, Color.grey);
        }

        public static void DrawBugReportButton()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
        
            if (DrawUnderlinedButton("Report a Bug",80f))
            {
                Application.OpenURL("https://github.com/phanphantz/ThaiTextCare-for-Unity/issues/new");
            }
            EditorGUILayout.EndHorizontal();
        }

        static bool DrawUnderlinedButton(string buttonName, float width)
        {
            var style = new GUIStyle(EditorStyles.linkLabel);
            style.normal.textColor = new Color(0.5f,0.5f,0.5f);
            if (GUILayout.Button(buttonName, style, GUILayout.Width(width)))
            {
                return true;
            }
            var r = GUILayoutUtility.GetLastRect();
            r.height = 1;
            r.width = width -4f;
            r.y+= EditorGUIUtility.singleLineHeight;
            EditorGUI.DrawRect(r, Color.grey);
            return false;
        }
    }
}