public class Landmark : EnergyConsumer
{
	protected override bool CanSelectOutput()
	{
		return false;
	}

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (Id == "laboratory" && (otherCard.Id == "science" || otherCard.Id == "fossil"))
		{
			return true;
		}
		return base.CanHaveCard(otherCard);
	}
}
