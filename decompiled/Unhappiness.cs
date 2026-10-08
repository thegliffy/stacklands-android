public class Unhappiness : CardData
{
	protected override bool CanHaveCard(CardData otherCard)
	{
		if (!(otherCard is Happiness))
		{
			return otherCard is Unhappiness;
		}
		return true;
	}

	public override void OnInitialCreate()
	{
		if (WorldManager.instance.GetCardCount<Unhappiness>() >= 20 && WorldManager.instance.GetCardCount("sadness_demon") <= 0)
		{
			WorldManager.instance.QueueCutsceneIfNotQueued(Cutscenes.DemonOfSadness(), "sadness_demon");
		}
		base.OnInitialCreate();
	}
}
