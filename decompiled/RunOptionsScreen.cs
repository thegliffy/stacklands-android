using System.Collections.Generic;
using UnityEngine;

public class RunOptionsScreen : SokScreen
{
	public CustomButton ShortMoon;

	public CustomButton NormalMoon;

	public CustomButton LongMoon;

	public CustomButton PeacefulModeOn;

	public CustomButton PeacefulModeOff;

	public CustomButton PlayButton;

	public CustomButton BackButton;

	public CustomButton HappinessButton;

	public CustomButton DeathButton;

	public RectTransform CurseOptions;

	public MoonLength CurMoonLength = MoonLength.Normal;

	public bool PeacefulMode;

	public bool EnableGreed;

	public bool EnableHappiness;

	public bool EnableDeath;

	private void Start()
	{
		PlayButton.Clicked += delegate
		{
			TransitionScreen.instance.StartTransition(delegate
			{
				WorldManager.instance.CurrentRunOptions = new RunOptions
				{
					MoonLength = CurMoonLength,
					IsPeacefulMode = PeacefulMode,
					IsGreedEnabled = EnableGreed,
					IsDeathEnabled = EnableDeath,
					IsHappinessEnabled = EnableHappiness
				};
				WorldManager.instance.CurrentSave?.LastPlayedRound?.SavedBoosterBoxes?.Clear();
				foreach (BuyBoosterBox allBoosterBox in WorldManager.instance.AllBoosterBoxes)
				{
					allBoosterBox.StoredCostAmount = 0;
				}
				WorldManager.instance.CurrentRunVariables = new RunVariables();
				WorldManager.instance.RoundExtraKeyValues = new List<SerializedKeyValuePair>();
				CitiesManager.instance.Wellbeing = CitiesManager.instance.WellbeingStart;
				WorldManager.instance.StartNewRound();
				WorldManager.instance.Play();
			});
		};
		BackButton.Clicked += delegate
		{
		};
		ShortMoon.Clicked += delegate
		{
			CurMoonLength = MoonLength.Short;
		};
		NormalMoon.Clicked += delegate
		{
			CurMoonLength = MoonLength.Normal;
		};
		LongMoon.Clicked += delegate
		{
			CurMoonLength = MoonLength.Long;
		};
		PeacefulModeOn.Clicked += delegate
		{
			PeacefulMode = true;
		};
		PeacefulModeOff.Clicked += delegate
		{
			PeacefulMode = false;
		};
		HappinessButton.Clicked += delegate
		{
			EnableHappiness = !EnableHappiness;
		};
		DeathButton.Clicked += delegate
		{
			EnableDeath = !EnableDeath;
		};
	}

	private void Update()
	{
		ShortMoon.Image.color = ((CurMoonLength != 0) ? ColorManager.instance.BackgroundColor : ColorManager.instance.InactiveBackgroundColor);
		NormalMoon.Image.color = ((CurMoonLength != MoonLength.Normal) ? ColorManager.instance.BackgroundColor : ColorManager.instance.InactiveBackgroundColor);
		LongMoon.Image.color = ((CurMoonLength != MoonLength.Long) ? ColorManager.instance.BackgroundColor : ColorManager.instance.InactiveBackgroundColor);
		PeacefulModeOn.Image.color = ((!PeacefulMode) ? ColorManager.instance.BackgroundColor : ColorManager.instance.InactiveBackgroundColor);
		PeacefulModeOff.Image.color = (PeacefulMode ? ColorManager.instance.BackgroundColor : ColorManager.instance.InactiveBackgroundColor);
		CurseOptions.gameObject.SetActive(WorldManager.instance.IsSpiritDlcActive());
		SetCurseButton(DeathButton, WorldManager.instance.CurrentSave.FinishedDeath, EnableDeath, "label_enable_death_curse");
		SetCurseButton(HappinessButton, WorldManager.instance.CurrentSave.FinishedHappiness, EnableHappiness, "label_enable_happiness_curse");
		if (InputController.instance.CancelTriggered())
		{
			GameCanvas.instance.SetScreen<MainMenu>();
		}
	}

	private void SetCurseButton(CustomButton but, bool curseUnlocked, bool curseEnabled, string mainTerm)
	{
		if (curseUnlocked)
		{
			but.ButtonEnabled = true;
			but.TooltipText = "";
			but.TextMeshPro.text = SokLoc.Translate(mainTerm, LocParam.Create("on_off", YesNo(curseEnabled)));
		}
		else
		{
			but.ButtonEnabled = false;
			but.TextMeshPro.text = SokLoc.Translate("label_enable_curse_unknown");
			but.TooltipText = SokLoc.Translate("label_beat_curse_to_unlock");
		}
	}

	public static string YesNo(bool a)
	{
		if (!a)
		{
			return SokLoc.Translate("label_off");
		}
		return SokLoc.Translate("label_on");
	}
}
