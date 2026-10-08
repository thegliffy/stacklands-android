public class Happiness : CardData
{
	protected override bool CanHaveCard(CardData otherCard)
	{
		if (!(otherCard is Happiness) && !(otherCard is Unhappiness) && !(otherCard is BaseVillager))
		{
			return otherCard.Id == "plank";
		}
		return true;
	}
}
