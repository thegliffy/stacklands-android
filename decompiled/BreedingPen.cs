public class BreedingPen : CardData
{
	public override bool DetermineCanHaveCardsWhenIsRoot => true;

	protected override bool CanHaveCard(CardData otherCard)
	{
		switch (MyGameCard.GetChildCount())
		{
		case 0:
			if (otherCard is Animal animal)
			{
				return animal.IsBreedable;
			}
			return false;
		case 1:
			return MyGameCard.Child.CardData.Id == otherCard.Id;
		default:
			return false;
		}
	}

	public override void UpdateCard()
	{
		if (MyGameCard.GetChildCount() == 2)
		{
			MyGameCard.StartTimer(120f, BreedAnimals, SokLoc.Translate("action_breeding_status"), GetActionId("BreedAnimals"));
		}
		else if (MyGameCard.GetChildCount() > 2)
		{
			GameCard gameCard = MyGameCard.TryGetNthChild(3);
			if (gameCard != null)
			{
				gameCard.RemoveFromParent();
			}
			MyGameCard.CancelTimer(GetActionId("BreedAnimals"));
		}
		else
		{
			MyGameCard.CancelTimer(GetActionId("BreedAnimals"));
		}
		base.UpdateCard();
	}

	[TimedAction("breed_animals")]
	public void BreedAnimals()
	{
		CardData cardData = WorldManager.instance.CreateCard(base.transform.position, MyGameCard.Child.CardData.Id);
		WorldManager.instance.StackSendCheckTarget(MyGameCard, cardData.MyGameCard, OutputDir);
		GameCard child = MyGameCard.Child;
		if (child.Child != null)
		{
			GameCard child2 = child.Child;
			child2.RemoveFromStack();
			WorldManager.instance.StackSend(child2, OutputDir);
		}
		QuestManager.instance.SpecialActionComplete("breed_" + cardData.Id);
		child.RemoveFromStack();
		WorldManager.instance.StackSend(child, OutputDir);
	}
}
