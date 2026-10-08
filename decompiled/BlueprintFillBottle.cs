using System.Collections.Generic;
using System.Linq;

public class BlueprintFillBottle : Blueprint
{
	public override void BlueprintComplete(GameCard rootCard, List<GameCard> involvedCards, Subprint print)
	{
		if (print.RequiredCards.Contains("empty_bottle"))
		{
			Harvestable harvestable = involvedCards.Find((GameCard x) => x.CardData.Id == "spring")?.CardData as Harvestable;
			if (harvestable != null)
			{
				harvestable.Amount--;
				if (harvestable.Amount <= 0)
				{
					harvestable.MyGameCard.DestroyCard(spawnSmoke: true);
				}
			}
		}
		base.BlueprintComplete(rootCard, involvedCards, print);
	}
}
