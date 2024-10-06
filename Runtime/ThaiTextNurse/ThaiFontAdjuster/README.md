# ThaiFontAdjuster

Unity3D font renderer lacks support for
[GPOS and GSUB](https://www.microsoft.com/typography/otspec/gpos.htm).
Because Thai font heavily depends on these features, rendered image looks ugly without them.

This library gives workaround to render Thai font almost correctly.
Following image shows differences between results from original Unity3D and this library.
Position of tone mark, upper vowel and lower vowel would be adjusted by surrounding characters.

![Features](https://raw.githubusercontent.com/SaladbowlCreative/Unity3D.ThaiFontAdjuster/master/doc/Features.png)

## Where can I get it?

Visit [Release](https://github.com/SaladbowlCreative/Unity3D.ThaiFontAdjuster/releases)
page to get latest ThaiFontAdjuster unity-package.

## How to use

Just before setting text of UnityEngine.UI.Text, text need to be translated by
`ThaiFontAdjuster.Adjust`. Font of Text should be one of supported Thai fonts.

```csharp
var s = "Thai ก์กิ์ป์ปิ์ฎุญุ";
Label.text = ThaiFontAdjuster.Adjust(s);
```

For other UI library using Unity3D dynamic font (like NGUI), you can do it by same way.

## Limitation

- ThaiFontAdjuster only can handle specific fonts.
  - Font should have extended glyphs from U+F700 to U+F71A providing various position of Thai characters.
  - [NECTEC National Fonts](http://www.nectec.or.th/pub/review-software/font/national-fonts.html) (Garuda, Loma, Kinnari, Norasi) already provide it.
  - Extended NotoSansThai-Regular for being supported is included in this package.
  - If you want to use another font, check it contains extended glyphs otherwise these glyphs have to be added.
- Adjusting position is not same as true-type rendering with GPOS and GSUB support.
  - This library adjusts position of glyph at best with limited extended set of character.
    But without GPOS, ideal positioning is impossibe.
