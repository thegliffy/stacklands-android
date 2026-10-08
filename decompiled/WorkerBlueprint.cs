using System.Collections.Generic;

public class WorkerBlueprint : Blueprint
{
	public override void BlueprintComplete(GameCard rootCard, List<GameCard> involvedCards, Subprint print)
	{
		CardData cardData = WorldManager.instance.CreateCard(rootCard.transform.position, print.ResultCard, faceUp: false, checkAddToStack: false);
		Apartment apartment = null;
		for (int num = involvedCards.Count - 1; num >= 0; num--)
		{
			GameCard gameCard = involvedCards[num];
			gameCard.RemoveFromStack();
			if (gameCard.CardData is Apartment)
			{
				apartment = (Apartment)gameCard.CardData;
			}
			else
			{
				gameCard.SendIt();
			}
		}
		if (apartment != null)
		{
			cardData.MyGameCard.SetParent(apartment.MyGameCard);
		}
	}
}
