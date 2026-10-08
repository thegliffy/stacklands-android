using UnityEngine;

public class AdvancedSettingsScreen : SokScreen
{
	public CustomButton BackButton;

	public CustomButton AdvancedCombatStatsButton;

	public static bool AdvancedCombatStatsEnabled;

	private void Awake()
	{
		AdvancedCombatStatsButton.Clicked += delegate
		{
			AdvancedCombatStatsEnabled = !AdvancedCombatStatsEnabled;
			SaveSettings();
		};
		BackButton.Clicked += delegate
		{
			GameCanvas.instance.SetScreen<OptionsScreen>();
		};
		LoadSettings();
	}

	private void Update()
	{
		AdvancedCombatStatsButton.TextMeshPro.text = SokLoc.Translate("label_advanced_combat", LocParam.Create("on_off", OptionsScreen.YesNo(AdvancedCombatStatsEnabled)));
	}

	private void OnApplicationQuit()
	{
		SaveSettings();
	}

	private void LoadSettings()
	{
		AdvancedCombatStatsEnabled = PlayerPrefs.GetInt("AdvancedCombatStatsEnabled", 0) == 1;
	}

	private void SaveSettings()
	{
		PlayerPrefs.SetInt("AdvancedCombatStatsEnabled", AdvancedCombatStatsEnabled ? 1 : 0);
	}
}
