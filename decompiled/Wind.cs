public class Wind : Weather
{
	protected override bool CanHaveCard(CardData otherCard)
	{
		if (!(otherCard is Worker) && !(otherCard.Id == "metal_scraps") && !(otherCard.Id == "factory_parts"))
		{
			return otherCard.Id == "wind";
		}
		return true;
	}
}
