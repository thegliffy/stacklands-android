using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsScreen : SokScreen
{
	public static int CurrentWidth;

	public static int CurrentHeight;

	public static bool CurrentFullScreen;

	public static int CurrentFrameRate;

	public static Vector2 CurrentUIScale;

	public const bool DEBUG_RESOLUTION = false;

	public CustomButton ResolutionButton;

	public CustomButton FullscreenButton;

	public CustomButton UIScaleButton;

	public CustomButton FrameRateCapButton;

	public CustomButton LanguageButton;

	public CustomButton ClearSaveButton;

	public CustomButton CreditsButton;

	public CustomButton ControlsButton;

	public CustomButton AccessibilityButton;

	public CustomButton BackButton;

	public CustomButton AdvancedSettingsButton;

	public Slider MusicSlider;

	public Slider SfxSlider;

	public CanvasScaler CanvasScaler;

	public TextMeshProUGUI VersionText;

	public TextMeshProUGUI MusicVolumeText;

	public TextMeshProUGUI SfxVolumeText;

	public CustomButton SelectSaveButton;

	private static List<int> frameRates = new List<int> { -2, -1, 30, 60, 120 };

	private static List<Vector2> UIScale = new List<Vector2>
	{
		new Vector2(1920f, 1080f),
		new Vector2(2500f, 1080f),
		new Vector2(3440f, 2169f)
	};

	public static bool MusicOn;

	public static bool SfxOn;

	public static float MusicVol;

	public static float SfxVol;

	public override bool IsFrameRateUncapped => true;

	private void Awake()
	{
		SelectSaveButton.Clicked += delegate
		{
			GameCanvas.instance.SetScreen<SelectSaveScreen>();
		};
		ResolutionButton.Clicked += delegate
		{
			GameCanvas.instance.SetScreen<SelectResolutionScreen>();
		};
		ControlsButton.Clicked += delegate
		{
			GameCanvas.instance.SetScreen<ControlsScreen>();
		};
		AccessibilityButton.Clicked += delegate
		{
			GameCanvas.instance.SetScreen<AccessibilityScreen>();
		};
		AdvancedSettingsButton.Clicked += delegate
		{
			GameCanvas.instance.SetScreen<AdvancedSettingsScreen>();
		};
		FullscreenButton.Clicked += ToggleFullScreen;
		FrameRateCapButton.Clicked += ToggleFrameRateCap;
		UIScaleButton.Clicked += ToggleUIScale;
		ClearSaveButton.Clicked += delegate
		{
			GameCanvas.instance.ShowClearSaveModal();
		};
		LanguageButton.Clicked += delegate
		{
			GameCanvas.instance.SetScreen<SelectLanguageScreen>();
		};
		MusicSlider.onValueChanged.AddListener(OnMusicVolumeChange);
		SfxSlider.onValueChanged.AddListener(OnSFXVolumeChange);
		CreditsButton.Clicked += delegate
		{
			GameCanvas.instance.SetScreen<CreditsScreen>();
		};
		BackButton.Clicked += delegate
		{
			GoBack();
		};
		LoadSettings();
		MusicSlider.value = MusicVol;
		SfxSlider.value = SfxVol;
		SokLoc.instance.LanguageChanged += Instance_LanguageChanged;
	}

	public void OnMusicVolumeChange(float sliderValue)
	{
		MusicVol = sliderValue;
	}

	public void OnSFXVolumeChange(float sliderValue)
	{
		SfxVol = sliderValue;
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
		SetTexts();
	}

	private void GoBack()
	{
		if (WorldManager.instance.CurrentGameState == WorldManager.GameState.Paused)
		{
			GameCanvas.instance.SetScreen<PauseScreen>();
		}
		else
		{
			GameCanvas.instance.SetScreen<MainMenu>();
		}
		SaveSettings();
	}

	private void Start()
	{
		SetTexts();
	}

	private static void ToggleUIScale()
	{
		int num = UIScale.IndexOf(CurrentUIScale) + 1;
		if (num == UIScale.Count)
		{
			num = 0;
		}
		CurrentUIScale = UIScale[num];
		SetUIScale();
	}

	private static void ToggleFullScreen()
	{
		CurrentFullScreen = !CurrentFullScreen;
		SetResolution();
	}

	private void ToggleFrameRateCap()
	{
		int num = frameRates.IndexOf(CurrentFrameRate) + 1;
		if (num == frameRates.Count)
		{
			num = 0;
		}
		CurrentFrameRate = frameRates[num];
	}

	private void Update()
	{
		VersionText.text = "v" + Application.version;
		if (InputController.instance.CancelTriggered() && !GameCanvas.instance.ModalIsOpen)
		{
			GoBack();
		}
		SetTexts();
	}

	private void OnDisable()
	{
		SaveSettings();
	}

	private void SetTexts()
	{
		ResolutionButton.TextMeshPro.text = SokLoc.Translate("label_resolution", LocParam.Create("width", CurrentWidth.ToString()), LocParam.Create("height", CurrentHeight.ToString()));
		FullscreenButton.TextMeshPro.text = SokLoc.Translate("label_fullscreen", LocParam.Create("on_off", YesNo(CurrentFullScreen)));
		FrameRateCapButton.TextMeshPro.text = SokLoc.Translate("label_framerate_cap", LocParam.Create("fps_cap", FramerateLabel(CurrentFrameRate)));
		UIScaleButton.TextMeshPro.text = SokLoc.Translate("label_ui_scale", LocParam.Create("scale", UIScaleLabel(UIScale.IndexOf(CurrentUIScale))));
		BackButton.TextMeshPro.text = SokLoc.Translate("label_back");
		MusicVolumeText.text = $"{Mathf.RoundToInt(MusicVol * 100f)}%";
		SfxVolumeText.text = $"{Mathf.RoundToInt(SfxVol * 100f)}%";
	}

	public static string YesNo(bool a)
	{
		if (!a)
		{
			return SokLoc.Translate("label_off");
		}
		return SokLoc.Translate("label_on");
	}

	public static string FramerateLabel(int i)
	{
		return i switch
		{
			-2 => SokLoc.Translate("label_framerate_unlimited"), 
			-1 => SokLoc.Translate("label_framerate_vsync"), 
			30 => SokLoc.Translate("label_framerate_30"), 
			60 => SokLoc.Translate("label_framerate_60"), 
			120 => SokLoc.Translate("label_framerate_120"), 
			_ => SokLoc.Translate("label_framerate_unlimited"), 
		};
	}

	public static string UIScaleLabel(int i)
	{
		return i switch
		{
			0 => SokLoc.Translate("label_ui_scale_100"), 
			1 => SokLoc.Translate("label_ui_scale_80"), 
			2 => SokLoc.Translate("label_ui_scale_60"), 
			_ => SokLoc.Translate("label_ui_scale_100"), 
		};
	}

	public static void LoadSettings()
	{
		MusicOn = PlayerPrefs.GetInt("musicOn", 1) == 1;
		SfxOn = PlayerPrefs.GetInt("sfxOn", 1) == 1;
		MusicVol = PlayerPrefs.GetFloat("MusicVolume", 0.8f);
		SfxVol = PlayerPrefs.GetFloat("SfxVolume", 0.8f);
		string languageName = SokLoc.DetermineSystemLanguage().LanguageName;
		string @string = PlayerPrefs.GetString("language", languageName);
		SokLoc.instance.SetLanguage(@string);
		int @int = PlayerPrefs.GetInt("width", -1);
		int int2 = PlayerPrefs.GetInt("height", -1);
		CurrentFullScreen = PlayerPrefs.GetInt("fullscreen", 1) == 1;
		CurrentFrameRate = PlayerPrefs.GetInt("framerate", -1);
		int int3 = PlayerPrefs.GetInt("uiScaleIndex", 0);
		if (int3 >= 0 && int3 < UIScale.Count)
		{
			CurrentUIScale = UIScale[int3];
		}
		else
		{
			CurrentUIScale = UIScale[0];
		}
		SetUIScale();
		if (@int != -1 && int2 != -1)
		{
			CurrentWidth = @int;
			CurrentHeight = int2;
			SetResolution();
			Debug.Log($"Loaded resolution {CurrentWidth}x{CurrentHeight}");
		}
		else
		{
			CurrentWidth = Screen.currentResolution.width;
			CurrentHeight = Screen.currentResolution.height;
			Debug.Log($"Set current resolution to {CurrentWidth}x{CurrentHeight}");
		}
	}

	public static void SaveSettings()
	{
		PlayerPrefs.SetInt("musicOn", MusicOn ? 1 : 0);
		PlayerPrefs.SetInt("sfxOn", SfxOn ? 1 : 0);
		PlayerPrefs.SetFloat("MusicVolume", MusicVol);
		PlayerPrefs.SetFloat("SfxVolume", SfxVol);
		PlayerPrefs.SetInt("fullscreen", CurrentFullScreen ? 1 : 0);
		PlayerPrefs.SetInt("framerate", CurrentFrameRate);
		PlayerPrefs.SetInt("uiScaleIndex", UIScale.IndexOf(CurrentUIScale));
		PlayerPrefs.SetInt("width", CurrentWidth);
		PlayerPrefs.SetInt("height", CurrentHeight);
		PlayerPrefs.SetString("language", SokLoc.instance.CurrentLanguage);
	}

	private static bool IsSameResolution(Resolution a, Resolution b)
	{
		if (a.width == b.width)
		{
			return a.height == b.height;
		}
		return false;
	}

	public static List<Resolution> PossibleResolutions()
	{
		List<Resolution> list = new List<Resolution>();
		List<Resolution> list2 = Screen.resolutions.ToList();
		list2.Sort((Resolution a, Resolution b) => a.width - b.width);
		for (int i = 0; i < list2.Count; i++)
		{
			Resolution resolution = list2[i];
			bool flag = false;
			for (int j = 0; j < list.Count; j++)
			{
				if (IsSameResolution(resolution, list[j]))
				{
					flag = true;
					break;
				}
			}
			if (!flag && resolution.height != 0 && resolution.width != 0)
			{
				list.Add(resolution);
			}
		}
		return list;
	}

	public static void SetUIScale()
	{
		GameCanvas.instance.Canvas.GetComponent<CanvasScaler>().referenceResolution = CurrentUIScale;
	}

	public static void SetResolution()
	{
		CurrentWidth = ((CurrentWidth < 100) ? PossibleResolutions()[0].width : CurrentWidth);
		CurrentHeight = ((CurrentHeight < 100) ? PossibleResolutions()[0].height : CurrentHeight);
		Screen.SetResolution(CurrentWidth, CurrentHeight, CurrentFullScreen);
	}
}
