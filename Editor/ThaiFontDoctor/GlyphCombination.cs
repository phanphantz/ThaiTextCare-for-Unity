using System;

namespace PhEngine.ThaiTextCare.Editor
{
    [Serializable]
    public class GlyphCombination
    {
        public bool isEnabled = true;
        public GlyphGroup first;
        public GlyphGroup second;

        public GlyphCombination()
        {
            isEnabled = true;
            first = new GlyphGroup();
            second = new GlyphGroup();
        }

        public GlyphCombination(GlyphCombination combo)
        {
            first = new GlyphGroup(combo.first);
            second = new GlyphGroup(combo.second);
        }

        public string DisplayName => first.DisplayName + " + " + second.DisplayName;

        public int VariantCount
        {
            get
            {
                if (first == null || second == null)
                    return 0;

                return first.GlyphCount * second.GlyphCount;
            }
        }
    }
}