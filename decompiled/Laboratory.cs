public class Laboratory : EnergyHarvestable
{
	public bool InCutscene;

	public override void UpdateCard()
	{
		if (HasCardOnTop("fossil", out var cardData) && !InCutscene)
		{
			InCutscene = true;
			WorldManager.instance.QueueCutscene(CitiesCutscenes.DinoBoss(this, cardData));
		}
		base.UpdateCard();
	}

	public override bool CanHaveCardsWhileHasStatus()
	{
		return true;
	}

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (!(otherCard.Id == "science"))
		{
			return otherCard.Id == "fossil";
		}
		return true;
	}
}
