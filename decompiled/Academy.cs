public class Academy : Landmark
{
	public float EducationTime = 120f;

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (otherCard.Id == "alien")
		{
			return false;
		}
		if (otherCard is Worker worker && (worker.GetWorkerType() == WorkerType.Educated || worker.GetWorkerType() == WorkerType.Robot) && CitiesManager.instance.Wellbeing >= 50)
		{
			return true;
		}
		if (otherCard is Worker worker2)
		{
			return worker2.GetWorkerType() == WorkerType.Normal;
		}
		return false;
	}

	public override void UpdateCard()
	{
		if (MyGameCard.HasChild && AllChildrenMatchPredicate((CardData x) => x is Worker))
		{
			Worker worker = MyGameCard.Child.CardData as Worker;
			if (((worker.Id == "educated_worker" || worker.Id == "genius") && WorldManager.instance.GetCardCount("genius") > 0) || ((worker.Id == "robot_worker" || worker.Id == "robot_genius") && WorldManager.instance.GetCardCount("robot_genius") > 0))
			{
				return;
			}
			if (!MyGameCard.TimerRunningInStack)
			{
				MyGameCard.StartTimer(EducationTime, EducateWorkers, SokLoc.Translate("card_academy_status"), GetActionId("EducateWorkers"));
			}
		}
		else
		{
			MyGameCard.CancelTimer(GetActionId("EducateWorkers"));
		}
		base.UpdateCard();
	}

	[TimedAction("educate_workers")]
	public void EducateWorkers()
	{
		GameCard leafCard = MyGameCard.GetLeafCard();
		if (leafCard.CardData is Worker)
		{
			if (leafCard.CardData.Id == "worker")
			{
				WorldManager.instance.ChangeToCard(leafCard, "educated_worker");
				leafCard.RemoveFromStack();
				leafCard.SendIt();
			}
			else if (leafCard.CardData.Id == "robot_worker" && CitiesManager.instance.Wellbeing >= 50 && WorldManager.instance.GetCardCount("robot_genius") == 0)
			{
				WorldManager.instance.ChangeToCard(leafCard, "robot_genius");
				leafCard.RemoveFromStack();
				leafCard.SendIt();
			}
			else if (leafCard.CardData.Id == "educated_worker" && CitiesManager.instance.Wellbeing >= 50 && WorldManager.instance.GetCardCount("genius") == 0)
			{
				WorldManager.instance.ChangeToCard(leafCard, "genius");
				leafCard.RemoveFromStack();
				leafCard.SendIt();
			}
		}
	}
}
