using System.Collections.Generic;
using UnityEngine;

public class SelectResolutionScreen : SokScreen
{
	public RectTransform ButtonsParent;

	public CustomButton BackButton;

	private List<CustomButton> resolutionButtons = new List<CustomButton>();

	private List<Resolution> resolutions = new List<Resolution>();

	private int lastScreenWidth;

	private int lastHighestWidth;

	private void Start()
	{
		BackButton.Clicked += delegate
		{
			GameCanvas.instance.SetScreen<OptionsScreen>();
		};
		lastScreenWidth = Screen.width;
	}

	private void Update()
	{
		if (lastScreenWidth != Screen.width && lastHighestWidth != Screen.currentResolution.width && resolutions != OptionsScreen.PossibleResolutions())
		{
			ResetResolutions();
		}
		lastHighestWidth = Screen.resolutions[Screen.resolutions.Length - 1].width;
		lastScreenWidth = Screen.width;
		resolutions = OptionsScreen.PossibleResolutions();
	}

	private void OnEnable()
	{
		ResetResolutions();
	}

	private void InitButtons()
	{
		Debug.Log("Reset resolutions");
		List<Resolution> list = OptionsScreen.PossibleResolutions();
		for (int i = 0; i < list.Count; i++)
		{
			Resolution res = list[i];
			CustomButton customButton = Object.Instantiate(PrefabManager.instance.ButtonPrefab);
			customButton.transform.SetParent(ButtonsParent);
			customButton.transform.localScale = Vector3.one;
			customButton.transform.localPosition = Vector3.zero;
			customButton.transform.localRotation = Quaternion.identity;
			customButton.TextMeshPro.text = res.width + "x" + res.height;
			customButton.Clicked += delegate
			{
				OptionsScreen.CurrentWidth = res.width;
				OptionsScreen.CurrentHeight = res.height;
				OptionsScreen.SetResolution();
			};
			resolutionButtons.Add(customButton);
		}
	}

	public void ResetResolutions()
	{
		foreach (CustomButton resolutionButton in resolutionButtons)
		{
			Object.Destroy(resolutionButton.gameObject);
		}
		resolutionButtons.Clear();
		InitButtons();
	}
}
