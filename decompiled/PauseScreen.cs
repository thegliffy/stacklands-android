using UnityEngine;

public class PauseScreen : SokScreen
{
	public CustomButton ContinueButton;

	public CustomButton CardopediaButton;

	public CustomButton OptionsButton;

	public CustomButton MainMenuButton;

	public CustomButton AbandonCityButton;

	public RectTransform NewCardopediaEntryRect;

	public override bool IsFrameRateUncapped => true;

	private void Awake()
	{
		ContinueButton.Clicked += delegate
		{
			WorldManager.instance.TogglePause();
		};
		OptionsButton.Clicked += delegate
		{
			GameCanvas.instance.SetScreen<OptionsScreen>();
		};
		AbandonCityButton.Clicked += delegate
		{
			WorldManager.instance.ModalAbandonCity();
		};
		CardopediaButton.Clicked += delegate
		{
			GameCanvas.instance.SetScreen<CardopediaScreen>();
		};
		MainMenuButton.Clicked += delegate
		{
			TransitionScreen.instance.StartTransition(delegate
			{
				SaveManager.instance.Save(saveRound: true);
				WorldManager.RestartGame();
			});
		};
		NewCardopediaEntryRect.gameObject.SetActive(value: false);
	}

	private void Update()
	{
		NewCardopediaEntryRect.gameObject.SetActive(WorldManager.instance.CurrentSave.NewCardopediaIds.Count > 0);
		if (WorldManager.instance.GetCurrentBoardSafe().Id == "cities")
		{
			AbandonCityButton.gameObject.SetActive(value: true);
		}
		else
		{
			AbandonCityButton.gameObject.SetActive(value: false);
		}
	}
}
