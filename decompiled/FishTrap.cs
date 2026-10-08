using System.Collections.Generic;
using System.Linq;

public class FishTrap : CardData
{
	public BaitBag DefaultBaitBag;

	public List<BaitBag> BaitBags;

	public float FishTime = 30f;

	public override bool DetermineCanHaveCardsWhenIsRoot => true;

	public override bool CanHaveCardsWhileHasStatus()
	{
		return true;
	}

	protected override bool CanHaveCard(CardData otherCard)
	{
		return otherCard is Food;
	}

	public override void UpdateCard()
	{
		if (HasCardOnTop(out Food _))
		{
			MyGameCard.StartTimer(FishTime, CompleteFishing, SokLoc.Translate("card_fish_trap_status"), "complete_fishing");
		}
		else
		{
			MyGameCard.CancelTimer("complete_fishing");
		}
		base.UpdateCard();
	}

	[TimedAction("complete_fishing")]
	public void CompleteFishing()
	{
		HasCardOnTop(out Food food);
		BaitBag baitBag = BaitBags.FirstOrDefault((BaitBag x) => x.BaitId == food.Id);
		if (baitBag == null)
		{
			baitBag = DefaultBaitBag;
		}
		ICardId cardId = baitBag.GetCard(removeCard: false);
		if (cardId == null)
		{
			cardId = (CardId)"cod";
		}
		CardData cardData = WorldManager.instance.CreateCard(base.transform.position, cardId, faceUp: false, checkAddToStack: false);
		WorldManager.instance.StackSendCheckTarget(MyGameCard, cardData.MyGameCard, OutputDir);
		DestroyChildrenMatchingPredicateAndRestack((CardData c) => c == food, 1);
	}
}
