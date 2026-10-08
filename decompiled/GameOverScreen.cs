using TMPro;
using UnityEngine;

public class GameOverScreen : SokScreen
{
	public CustomButton BackButton;

	public TextMeshProUGUI StatsText;

	private float timer;

	private void Awake()
	{
		BackButton.Clicked += delegate
		{
			TransitionScreen.instance.StartTransition(delegate
			{
				WorldManager.instance.ClearRoundAndRestart();
			});
		};
	}

	private void OnEnable()
	{
		StatsText.maxVisibleLines = 0;
	}

	private void Update()
	{
		string text = "";
		text = text + SokLoc.Translate("label_you_reached_moon", LocParam.Create("moon", WorldManager.instance.CurrentMonth.ToString())) + "\n";
		text = text + SokLoc.Translate("label_quests_completed", LocParam.Plural("count", WorldManager.instance.QuestsCompleted)) + "\n";
		text = text + SokLoc.Translate("label_new_cards_found", LocParam.Plural("count", WorldManager.instance.NewCardsFound)) + "\n";
		StatsText.text = text;
		timer += Time.deltaTime;
		if (timer >= 0.3f)
		{
			timer = 0f;
			StatsText.maxVisibleLines++;
		}
	}
}
