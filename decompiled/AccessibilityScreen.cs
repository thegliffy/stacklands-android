using UnityEngine;

public class AccessibilityScreen : SokScreen
{
	public CustomButton BackButton;

	public CustomButton AutoPauseWhenUsingControllerButton;

	public CustomButton AutoPauseWhenUsingKeyboardMouseButton;

	public CustomButton ScreenShakeButton;

	public CustomButton ClickToDragButton;

	public CustomButton DisablePausedTextButton;

	public static bool AutoPauseWhenUsingController = true;

	public static bool AutoPauseWhenUsingKeyboardMouse = false;

	public static bool ScreenshakeEnabled = true;

	public static bool ClickToDragEnabled = false;

	public static bool FlashingPausedEnabled = true;

	private void Awake()
	{
		AutoPauseWhenUsingControllerButton.Clicked += delegate
		{
			AutoPauseWhenUsingController = !AutoPauseWhenUsingController;
			SaveSettings();
		};
		AutoPauseWhenUsingKeyboardMouseButton.Clicked += delegate
		{
			AutoPauseWhenUsingKeyboardMouse = !AutoPauseWhenUsingKeyboardMouse;
			SaveSettings();
		};
		ScreenShakeButton.Clicked += delegate
		{
			ScreenshakeEnabled = !ScreenshakeEnabled;
			SaveSettings();
		};
		ClickToDragButton.Clicked += delegate
		{
			ClickToDragEnabled = !ClickToDragEnabled;
			SaveSettings();
		};
		DisablePausedTextButton.Clicked += delegate
		{
			FlashingPausedEnabled = !FlashingPausedEnabled;
			SaveSettings();
		};
		BackButton.Clicked += delegate
		{
			GameCanvas.instance.SetScreen<OptionsScreen>();
		};
		LoadSettings();
	}

	private void OnApplicationQuit()
	{
		SaveSettings();
	}

	private void LoadSettings()
	{
		AutoPauseWhenUsingController = PlayerPrefs.GetInt("AutoPauseWithController", 1) == 1;
		AutoPauseWhenUsingKeyboardMouse = PlayerPrefs.GetInt("AutoPauseWithKeyboardMouse", 0) == 1;
		ScreenshakeEnabled = PlayerPrefs.GetInt("ScreenshakeEnabled", 1) == 1;
		ClickToDragEnabled = PlayerPrefs.GetInt("ClickToDragEnabled", 0) == 1;
		FlashingPausedEnabled = PlayerPrefs.GetInt("FlashingPausedEnabled", 0) == 1;
	}

	private void SaveSettings()
	{
		PlayerPrefs.SetInt("AutoPauseWithController", AutoPauseWhenUsingController ? 1 : 0);
		PlayerPrefs.SetInt("AutoPauseWithKeyboardMouse", AutoPauseWhenUsingKeyboardMouse ? 1 : 0);
		PlayerPrefs.SetInt("ScreenshakeEnabled", ScreenshakeEnabled ? 1 : 0);
		PlayerPrefs.SetInt("ClickToDragEnabled", ClickToDragEnabled ? 1 : 0);
		PlayerPrefs.SetInt("FlashingPausedEnabled", FlashingPausedEnabled ? 1 : 0);
	}

	private void Update()
	{
		AutoPauseWhenUsingControllerButton.TextMeshPro.text = SokLoc.Translate("label_auto_pause_controller") + " " + OptionsScreen.YesNo(AutoPauseWhenUsingController);
		AutoPauseWhenUsingKeyboardMouseButton.TextMeshPro.text = SokLoc.Translate("label_auto_pause_keyboardmouse") + " " + OptionsScreen.YesNo(AutoPauseWhenUsingKeyboardMouse);
		ScreenShakeButton.TextMeshPro.text = SokLoc.Translate("label_screenshake_enabled") + ": " + OptionsScreen.YesNo(ScreenshakeEnabled);
		ClickToDragButton.TextMeshPro.text = SokLoc.Translate("label_clicktodrag_enabled") + ": " + OptionsScreen.YesNo(ClickToDragEnabled);
		DisablePausedTextButton.TextMeshPro.text = SokLoc.Translate("label_disable_paused_text", LocParam.Create("on_off", OptionsScreen.YesNo(FlashingPausedEnabled)));
		ClickToDragButton.TooltipText = SokLoc.Translate("label_clicktodrag_tooltip");
	}
}
