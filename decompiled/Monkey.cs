public class Monkey : Animal
{
	protected override bool CanHaveCard(CardData otherCard)
	{
		if (otherCard.Id == "banana")
		{
			return true;
		}
		return base.CanHaveCard(otherCard);
	}

	public override void UpdateCard()
	{
		if (MyGameCard.HasChild && MyGameCard.Child.CardData.Id == "banana")
		{
			MyGameCard.StartTimer(1f, TrainMonkey, SokLoc.Translate("idea_training_monkey_status"), GetActionId("TrainMonkey"));
		}
		else
		{
			MyGameCard.CancelTimer(GetActionId("TrainMonkey"));
		}
		base.UpdateCard();
	}

	[TimedAction("train_monkey")]
	public void TrainMonkey()
	{
		MyGameCard.Child.DestroyCard();
		WorldManager.instance.ChangeToCard(MyGameCard, "trained_monkey");
	}
}
