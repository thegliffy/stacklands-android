public class Cathedral : CardData
{
	protected override bool CanHaveCard(CardData otherCard)
	{
		return otherCard.Id == "island_relic";
	}

	public override void UpdateCard()
	{
		if (WorldManager.instance.IsPlaying && !WorldManager.instance.InAnimation && HasCardOnTop("island_relic", out var cardData))
		{
			QuestManager.instance.SpecialActionComplete("island_relic_to_cathedral", this);
			WorldManager.instance.QueueCutscene(Cutscenes.BossFight2(this, cardData));
		}
		base.UpdateCard();
	}
}
