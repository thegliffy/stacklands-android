using System.Collections.Generic;

public class BlueprintOffspring : Blueprint
{
	public List<Subprint> BaseSubprints;

	public List<Subprint> SpiritsSubprints;

	public override void Init(GameDataLoader loader)
	{
		base.Init(loader);
		Subprints = (loader.SpiritDlcLoaded ? SpiritsSubprints : BaseSubprints);
	}

	public override void BlueprintComplete(GameCard rootCard, List<GameCard> involvedCards, Subprint print)
	{
		CardData cardData = WorldManager.instance.CreateCard(rootCard.transform.position, print.ResultCard, faceUp: false, checkAddToStack: false);
		cardData.MyGameCard.SendIt();
		House house = null;
		for (int num = involvedCards.Count - 1; num >= 0; num--)
		{
			GameCard gameCard = involvedCards[num];
			gameCard.RemoveFromStack();
			if (gameCard.CardData is House)
			{
				house = (House)gameCard.CardData;
			}
			gameCard.SendIt();
		}
		if (house != null)
		{
			cardData.MyGameCard.SetParent(house.MyGameCard);
		}
	}
}
