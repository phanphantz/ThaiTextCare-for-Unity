using System;
using System.Linq;

namespace PhEngine.ThaiTextCare.Editor
{
    public static class ThaiGlyphHelper
    {
        static char[] behindDashGlyphs => allFollowingVowels
                .Concat(lowerVowels)
                .Concat(allFollowingVowels)
                .Concat(allUpperGlyphs)
                .ToArray();

        static readonly char[] lowerVowels = new[] { 'ุ', 'ู'};
        static readonly char[] allFollowingVowels = new[] {'ะ', 'ำ', 'า', 'ๅ'};
        static readonly char[] leadingVowels = new[] {'เ', 'แ', 'โ', 'ไ', 'ใ'};
        static readonly char[] upperVowels = new[] { 'ิ', 'ี', 'ึ', 'ื', '็', 'ั'};

        static readonly char[] allUpperGlyphs = new[] { 'ิ', 'ี', 'ึ', 'ื', '็', 'ั', '์', '่', '้', '๊', '๋'};
        static readonly char[] toneMarks = new[] {'่', '้', '๊', '๋'};
        static readonly char thanThaKhaat = '์';
        static readonly char saraAum = 'ำ';

        static readonly char[] allConsonants = new char[]
        {
            'ก', 'ข', 'ฃ', 'ค', 'ฅ', 'ฆ', 'ง', 'จ', 'ฉ', 'ช',
            'ซ', 'ฌ', 'ญ', 'ฎ', 'ฏ', 'ฐ', 'ฑ', 'ฒ', 'ณ', 'ด',
            'ต', 'ถ', 'ท', 'ธ', 'น', 'บ', 'ป', 'ผ', 'ฝ', 'พ',
            'ฟ', 'ภ', 'ม', 'ย', 'ร', 'ล', 'ว', 'ศ', 'ษ', 'ส',
            'ห', 'ฬ', 'อ', 'ฮ'
        };

        static readonly char[] descenderConsonants = new char[]
        {
            'ฎ', 'ฏ'
        };

        static readonly char[] ascenderConsonants = new char[]
        {
            'ป', 'ฝ', 'ฟ', 'ฬ'
        };

        public static string GetDisplayedString(string characters)
        {
            characters = characters.Trim();
            if (string.IsNullOrEmpty(characters))
                return "";
            
            if (characters.Length > 1)
                return characters;

            if (behindDashGlyphs.Contains(characters[0]))
                return "-" + characters[0];

            if (leadingVowels.Contains(characters[0]))
                return characters[0] + "-";

            return characters;
        }

        public static string GetThaiGlyphGroupName(ThaiGlyphPreset preset)
        {
            switch (preset)
            {
                case ThaiGlyphPreset.LowerVowels:
                    return "สระล่าง";
                case ThaiGlyphPreset.AllUpperGlyphs:
                    return "อักขระด้านบน";
                case ThaiGlyphPreset.AllFollowingVowels:
                    return "สระหลัง";
                case ThaiGlyphPreset.LeadingVowels:
                    return "สระหน้า";
                case ThaiGlyphPreset.UpperVowels:
                    return "สระบน";
                case ThaiGlyphPreset.ToneMarks:
                    return "วรรณยุกต์";
                case ThaiGlyphPreset.ThanThaKhaat:
                    return "ทัณฑฆาต";
                case ThaiGlyphPreset.SaraAum:
                    return "สระอำ";
                case ThaiGlyphPreset.AllConsonants:
                    return "ก - ฮ";
                case ThaiGlyphPreset.DescenderConsonants:
                    return "พยัญชนะหางล่าง";
                case ThaiGlyphPreset.AscenderConsonants:
                    return "พยัญชนะหางบน";
                case ThaiGlyphPreset.Custom:
                    return "กำหนดเอง";
                default:
                    throw new ArgumentOutOfRangeException(nameof(preset), preset, null);
            }
        }
        
        public static char[] GetGlyphsOf(ThaiGlyphPreset preset)
        {
            switch (preset)
            {
                case ThaiGlyphPreset.AllConsonants:
                    return allConsonants;
                case ThaiGlyphPreset.AscenderConsonants:
                    return ascenderConsonants;
                case ThaiGlyphPreset.DescenderConsonants:
                    return descenderConsonants;
                case ThaiGlyphPreset.AllUpperGlyphs:
                    return allUpperGlyphs;
                case ThaiGlyphPreset.UpperVowels:
                    return upperVowels;
                case ThaiGlyphPreset.ToneMarks:
                    return toneMarks;
                case ThaiGlyphPreset.ThanThaKhaat:
                    return new char[]{thanThaKhaat};
                case ThaiGlyphPreset.LeadingVowels:
                    return leadingVowels;
                case ThaiGlyphPreset.AllFollowingVowels:
                    return allFollowingVowels;
                case ThaiGlyphPreset.SaraAum:
                    return new char[]{saraAum};
                case ThaiGlyphPreset.LowerVowels:
                    return lowerVowels;
                case ThaiGlyphPreset.Custom:
                    return new char[] { };
                default:
                    throw new ArgumentOutOfRangeException(nameof(preset), preset, null);
            }
        }
    }

    public enum ThaiGlyphPreset
    {
        AllConsonants, 
        AscenderConsonants,
        DescenderConsonants, 
        AllUpperGlyphs, 
        UpperVowels, 
        ToneMarks, 
        ThanThaKhaat, 
        LeadingVowels, 
        AllFollowingVowels, 
        SaraAum, 
        LowerVowels, 
        Custom
    }
}