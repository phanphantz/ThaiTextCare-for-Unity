using System;
using System.Linq;

namespace PhEngine.ThaiTextCare.Editor
{
    [Serializable]
    public class GlyphGroup
    {
        public ThaiGlyphPreset preset;
        public string glyphs;
        public float xPlacement;
        public float yPlacement;
        
        public int GlyphCount => glyphs?.Length ?? 0;
        public string DisplayName
        {
            get
            {
                if (preset != ThaiGlyphPreset.Custom)
                    return ThaiGlyphHelper.GetThaiGlyphGroupName(preset);

                var displayedChars = glyphs.Select(c => ThaiGlyphHelper.GetDisplayedString(c.ToString())).ToArray();
                if (displayedChars.Length == 1)
                    return displayedChars.FirstOrDefault();

                return "[ " + string.Join(", ", displayedChars) + " ]";
            }
        }

        public GlyphGroup()
        {
            glyphs = "";
            preset = ThaiGlyphPreset.Custom;
        }

        public GlyphGroup(GlyphGroup group)
        {
            glyphs = group.glyphs;
            preset = group.preset;
            xPlacement = group.xPlacement;
            yPlacement = group.yPlacement;
        }

        public void AssignGroup(ThaiGlyphPreset value)
        {
            preset = value;
            var presetGlyphs = ThaiGlyphHelper.GetGlyphsOf(preset);
            if (presetGlyphs.Length > 0)
                glyphs = string.Join("", presetGlyphs);
        }
    }
}