public class Temple : CardData
{
	protected override bool CanHaveCard(CardData otherCard)
	{
		return otherCard.Id == "goblet";
	}

	public override void UpdateCard()
	{
		if (WorldManager.instance.IsPlaying && !WorldManager.instance.InAnimation && HasCardOnTop("goblet", out var cardData))
		{
			QuestManager.instance.SpecialActionComplete("goblet_to_temple", this);
			WorldManager.instance.QueueCutscene(Cutscenes.BossFight(this, cardData));
		}
		base.UpdateCard();
	}
}
