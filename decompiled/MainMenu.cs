using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainMenu : SokScreen
{
	public CustomButton NewGameButton;

	public CustomButton ContinueButton;

	public CustomButton CardopediaButton;

	public CustomButton OptionsButton;

	public CustomButton ModsButton;

	public CustomButton QuitButton;

	public CustomButton JoinDiscordButton;

	public CustomButton UpdateInfoButton;

	public TextMeshProUGUI UpdateText;

	public RectTransform CardopediaNewElement;

	public UpdatePopup UpdatePopup;

	public TextMeshProUGUI TitleText2000;

	private float timer;

	private void StartNewRun()
	{
		GameCanvas.instance.SetScreen<RunOptionsScreen>();
	}

	private void Awake()
	{
		JoinDiscordButton.Clicked += delegate
		{
			Application.OpenURL("https://discord.gg/sokpop");
		};
		NewGameButton.Clicked += delegate
		{
			if (WorldManager.instance.CurrentSave.LastPlayedRound != null)
			{
				GameCanvas.instance.ShowStartNewRunModal(delegate
				{
					StartNewRun();
				});
			}
			else
			{
				StartNewRun();
			}
		};
		ContinueButton.Clicked += delegate
		{
			HashSet<string> hashSet = WorldManager.instance.FindMissingCardsInSave();
			if (hashSet.Count == 0)
			{
				TransitionScreen.instance.StartTransition(delegate
				{
					WorldManager.instance.LoadPreviousRound();
					WorldManager.instance.Play();
				}, 1f);
			}
			else
			{
				GameCanvas.instance.MissingCardsInSavePrompt(delegate
				{
					TransitionScreen.instance.StartTransition(delegate
					{
						WorldManager.instance.LoadPreviousRound();
						WorldManager.instance.Play();
					}, 1f);
				}, hashSet);
			}
		};
		OptionsButton.Clicked += delegate
		{
			GameCanvas.instance.SetScreen<OptionsScreen>();
		};
		if (!PlatformHelper.HasModdingSupport)
		{
			ModsButton.gameObject.SetActive(value: false);
		}
		ModsButton.Clicked += delegate
		{
			GameCanvas.instance.SetScreen<ModsScreen>();
		};
		CardopediaButton.Clicked += delegate
		{
			GameCanvas.instance.SetScreen<CardopediaScreen>();
		};
		QuitButton.Clicked += delegate
		{
			Application.Quit();
		};
		UpdateInfoButton.Clicked += delegate
		{
			UpdatePopup.gameObject.SetActive(value: true);
		};
		UpdatePopup.gameObject.SetActive(value: false);
		if (PlayerPrefs.GetInt("showUpdatePopup", 4) <= 4)
		{
			UpdatePopup.gameObject.SetActive(value: true);
			PlayerPrefs.SetInt("showUpdatePopup", 5);
		}
		TitleText2000.gameObject.SetActive(WorldManager.instance.IsCitiesDlcActive());
	}

	private void Update()
	{
		timer += Time.deltaTime;
		if (InputController.instance.GetKeyDown(Key.Space) || InputController.instance.GetKeyDown(Key.Escape))
		{
			UpdatePopup.gameObject.SetActive(value: false);
		}
		JoinDiscordButton.gameObject.SetActive(GameCanvas.instance.ScreenIsInteractable<MainMenu>());
		CardopediaNewElement.gameObject.SetActive(WorldManager.instance.CurrentSave.NewCardopediaIds.Count > 0);
		if (WorldManager.instance.CurrentSave.LastPlayedRound != null)
		{
			string value = ((WorldManager.instance.CurrentSave.LastPlayedRound.SaveVersion <= 1) ? WorldManager.instance.CurrentSave.LastPlayedRound.CurrentMonth.ToString() : ((!(WorldManager.instance.CurrentSave.LastPlayedRound.CurrentBoardId == "cities")) ? (WorldManager.instance.CurrentSave.LastPlayedRound.BoardMonths.MainMonth + WorldManager.instance.CurrentSave.LastPlayedRound.BoardMonths.IslandMonth).ToString() : WorldManager.instance.CurrentSave.LastPlayedRound.BoardMonths.CitiesMonth.ToString()));
			ContinueButton.TextMeshPro.text = SokLoc.Translate("label_continue_run", LocParam.Create("moon", value));
		}
		if (WorldManager.instance.IsCitiesDlcActive())
		{
			UpdateText.text = SokLoc.Translate("label_menu_cities_title");
		}
		else
		{
			UpdateText.text = SokLoc.Translate("label_menu_cities_title_locked");
		}
		ContinueButton.gameObject.SetActive(WorldManager.instance.CurrentSave.LastPlayedRound != null);
		Vector3 b = ((!UpdateInfoButton.IsHovered) ? (Vector3.one * (1f + Mathf.Sin(timer * 2f) * 0.1f)) : new Vector3(1.2f, 1.2f, 1.2f));
		UpdateInfoButton.transform.localScale = Vector3.Lerp(UpdateInfoButton.transform.localScale, b, Time.deltaTime * 18f);
	}
}
