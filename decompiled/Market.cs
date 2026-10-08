public class Market : CardData
{
	public override bool DetermineCanHaveCardsWhenIsRoot => true;

	public override bool CanHaveCardsWhileHasStatus()
	{
		return true;
	}

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (otherCard.MyGameCard == null)
		{
			return otherCard.Value > 0;
		}
		return WorldManager.instance.CardCanBeSold(otherCard.MyGameCard);
	}

	public override void UpdateCard()
	{
		if (MyGameCard.HasChild && WorldManager.instance.CardCanBeSold(MyGameCard.Child, checkStatus: false) && (!MyGameCard.HasParent || MyGameCard.Parent.CardData is HeavyFoundation))
		{
			string status = SokLoc.Translate("new_selling_card", LocParam.Create("card", MyGameCard.Child.CardData.FullName));
			MyGameCard.StartTimer(60f, SellWithMarket, status, GetActionId("SellWithMarket"));
		}
		else
		{
			MyGameCard.CancelTimer(GetActionId("SellWithMarket"));
		}
		base.UpdateCard();
	}

	[TimedAction("sell_with_market")]
	public void SellWithMarket()
	{
		GameCard child = MyGameCard.Child;
		if (!(child == null))
		{
			GameCard gameCard = null;
			if (child.HasChild && WorldManager.instance.CardCanBeSold(child.Child))
			{
				gameCard = child.Child;
			}
			child.RemoveFromStack();
			if (gameCard != null)
			{
				MyGameCard.SetChild(gameCard);
			}
			QuestManager.instance.SpecialActionComplete("sell_at_market", this);
			GameCard gameCard2 = WorldManager.instance.SellCard(base.transform.position, child, 2f, checkAddToStack: false);
			if (gameCard2 != null)
			{
				WorldManager.instance.StackSendCheckTarget(MyGameCard, gameCard2.GetRootCard(), OutputDir, MyGameCard);
			}
		}
	}
}
