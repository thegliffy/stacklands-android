using TMPro;
using UnityEngine;

[ExecuteAlways]
public class FontSetter : MonoBehaviour
{
	public FontType MyFontType;

	[HideInInspector]
	public string LanguageOverride;

	private TextMeshPro tmPro;

	private void Start()
	{
		SetFont();
		if (SokLoc.instance != null)
		{
			SokLoc.instance.LanguageChanged += SetFont;
		}
	}

	private void OnEnable()
	{
		SetFont();
	}

	private void OnDestroy()
	{
		if (SokLoc.instance != null)
		{
			SokLoc.instance.LanguageChanged -= SetFont;
		}
	}

	private void Update()
	{
		if (!Application.isPlaying)
		{
			SetFont();
		}
		if (tmPro != null && FontManager.instance != null)
		{
			tmPro.fontSharedMaterial = FontManager.instance.WorldFontMaterial;
		}
	}

	private void SetFont()
	{
		if (!(FontManager.instance == null))
		{
			TMP_FontAsset font = FontManager.instance.GetFont(MyFontType, LanguageOverride);
			TextMeshProUGUI component = GetComponent<TextMeshProUGUI>();
			if (component != null)
			{
				component.font = font;
				component.material = font.material;
			}
			tmPro = GetComponent<TextMeshPro>();
			if (tmPro != null)
			{
				tmPro.font = font;
				tmPro.fontSharedMaterial = FontManager.instance.WorldFontMaterial;
			}
		}
	}
}
