using System.Collections.Generic;
using System.Linq;

public class ArtGallery : Landmark
{
	public int ArtPrice = 50;

	[Card]
	public List<string> AcceptedCards;

	protected override bool CanHaveCard(CardData otherCard)
	{
		return AcceptedCards.Contains(otherCard.Id);
	}

	public override bool CanHaveCardsWhileHasStatus()
	{
		return true;
	}

	public override void UpdateCard()
	{
		if (MyGameCard.HasChild && MyGameCard.Child.CardData is ICurrency)
		{
			if (ChildrenMatchingPredicate((CardData x) => x is ICurrency).Cast<ICurrency>().ToList().Sum((ICurrency x) => x.CurrencyValue) >= ArtPrice)
			{
				if (!MyGameCard.TimerRunning)
				{
					MyGameCard.StartTimer(60f, CreatePainting, SokLoc.Translate("card_art_gallery_status_1"), GetActionId("CreatePainting"));
				}
			}
			else
			{
				MyGameCard.CancelTimer(GetActionId("CreatePainting"));
			}
		}
		else
		{
			MyGameCard.CancelTimer(GetActionId("CreatePainting"));
		}
		base.UpdateCard();
	}

	[TimedAction("create_painting")]
	public void CreatePainting()
	{
		List<ICurrency> list = ChildrenMatchingPredicate((CardData x) => x is ICurrency).Cast<ICurrency>().ToList();
		if (list.Sum((ICurrency x) => x.CurrencyValue) >= ArtPrice)
		{
			CitiesManager.instance.TryUseDollars(list, ArtPrice, onlyTakeIfAmountMet: true, spawnSmoke: true, keepOnStack: true);
			CardData cardData = WorldManager.instance.CreateCard(base.transform.position, "artwork");
			cardData.MyGameCard.RemoveFromStack();
			WorldManager.instance.StackSend(cardData.MyGameCard, OutputDir);
		}
	}
}
