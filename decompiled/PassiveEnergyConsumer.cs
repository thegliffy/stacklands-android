public class PassiveEnergyConsumer : EnergyConsumer
{
	protected override bool CanHaveCard(CardData otherCard)
	{
		return true;
	}
}
