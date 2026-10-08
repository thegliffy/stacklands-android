public class SacredChest : CardData
{
	protected override bool CanHaveCard(CardData otherCard)
	{
		return otherCard.Id == "sacred_key";
	}

	public override void UpdateCard()
	{
		if (HasCardOnTop("sacred_key", out var cardData))
		{
			WorldManager.instance.CreateCard(base.transform.position, "island_relic", faceUp: false, checkAddToStack: false).MyGameCard.SendIt();
			QuestManager.instance.SpecialActionComplete("sacred_chest_opened", this);
			if (!WorldManager.instance.CurrentRunOptions.IsPeacefulMode)
			{
				WorldManager.instance.QueueCutscene(Cutscenes.SpawnTentacles());
			}
			else
			{
				WorldManager.instance.CurrentRunVariables.FinishedKraken = true;
			}
			cardData.MyGameCard.DestroyCard();
			MyGameCard.DestroyCard();
		}
		base.UpdateCard();
	}
}
