using UnityEngine;

public class SelectLanguageScreen : SokScreen
{
	public RectTransform ButtonsParent;

	public CustomButton BackButton;

	private void Start()
	{
		SokLanguage[] languages = SokLoc.Languages;
		foreach (SokLanguage language in languages)
		{
			CustomButton customButton = Object.Instantiate(PrefabManager.instance.ButtonPrefab);
			customButton.transform.SetParent(ButtonsParent);
			customButton.transform.localScale = Vector3.one;
			customButton.transform.localPosition = Vector3.zero;
			customButton.transform.localRotation = Quaternion.identity;
			customButton.name = language.LanguageName;
			customButton.TextMeshPro.text = SokLoc.GetLocalLanguageName(language.LanguageName);
			customButton.GetComponentInChildren<FontSetter>().LanguageOverride = language.LanguageName;
			customButton.Clicked += delegate
			{
				SokLoc.instance.SetLanguage(language.LanguageName);
				OptionsScreen.SaveSettings();
			};
		}
		BackButton.Clicked += delegate
		{
			GameCanvas.instance.SetScreen<OptionsScreen>();
		};
	}
}
