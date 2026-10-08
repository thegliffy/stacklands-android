using System;
using TMPro;
using UnityEngine;

[ExecuteInEditMode]
public class FontManager : MonoBehaviour
{
	public static FontManager instance;

	public TMP_FontAsset RegularFontAsset;

	public TMP_FontAsset TitleFontAsset;

	public TMP_FontAsset WorldFontAsset;

	public TMP_FontAsset ChineseTraditionalFontAsset;

	public TMP_FontAsset ChineseSimplifiedFontAsset;

	public TMP_FontAsset JapaneseFontAsset;

	public TMP_FontAsset KoreanFontAsset;

	public Material WorldFontMaterial;

	public Shader WorldFontShader;

	private void Awake()
	{
		instance = this;
		UpdateWorldFontMaterial();
		if (SokLoc.instance != null)
		{
			SokLoc.instance.LanguageChanged += Instance_LanguageChanged;
		}
	}

	private void Start()
	{
		UpdateWorldFontMaterial();
	}

	private void OnDestroy()
	{
		if (SokLoc.instance != null)
		{
			SokLoc.instance.LanguageChanged -= Instance_LanguageChanged;
		}
	}

	private void Instance_LanguageChanged()
	{
		UpdateWorldFontMaterial();
	}

	private void Update()
	{
		instance = this;
	}

	public void UpdateWorldFontMaterial()
	{
		if (SokLoc.instance != null)
		{
			TMP_FontAsset font = GetFont(FontType.World, SokLoc.instance.CurrentLanguage);
			WorldFontMaterial.CopyPropertiesFromMaterial(font.material);
			WorldFontMaterial.shader = WorldFontShader;
		}
	}

	public TMP_FontAsset GetFont(FontType fontType, string languageOverride = null)
	{
		if (SokLoc.instance != null)
		{
			string text = SokLoc.instance.CurrentLanguage;
			if (!string.IsNullOrEmpty(languageOverride))
			{
				text = languageOverride;
			}
			switch (text)
			{
			case "Chinese (Traditional)":
				return ChineseTraditionalFontAsset;
			case "Chinese (Simplified)":
				return ChineseSimplifiedFontAsset;
			case "Japanese":
				return JapaneseFontAsset;
			case "Korean":
				return KoreanFontAsset;
			}
		}
		return fontType switch
		{
			FontType.Regular => RegularFontAsset, 
			FontType.Rounded => TitleFontAsset, 
			FontType.World => WorldFontAsset, 
			_ => throw new ArgumentException("Unknown fontType"), 
		};
	}
}
