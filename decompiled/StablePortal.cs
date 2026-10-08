public class StablePortal : Portal
{
	public float TravelTime = 30f;

	public override void UpdateCard()
	{
		if (string.IsNullOrWhiteSpace(descriptionOverride))
		{
			descriptionOverride = SokLoc.Translate("card_stable_portal_description", LocParam.Create("amount", MaxVillagerCount.ToString()));
		}
		if (!TransitionScreen.InTransition && !WorldManager.instance.InAnimation)
		{
			int num = ChildrenMatchingPredicateCount((CardData x) => x is BaseVillager);
			if (num > 0)
			{
				if (!WorldManager.instance.CurrentBoard.BoardOptions.CanTravelToForest)
				{
					GameCanvas.instance.ShowCantChangeBoardSpirit();
					Stay();
					return;
				}
				RemoveNonHuman();
				int cardCount = WorldManager.instance.GetCardCount((CardData x) => x is BaseVillager);
				if (ChildrenMatchingPredicateCount((CardData x) => x is BaseVillager) > MaxVillagerCount && !GameCanvas.instance.ModalIsOpen)
				{
					MyGameCard.CancelTimer(GetActionId("TakePortal"));
					GameCanvas.instance.MaxVillagerCountPrompt("label_taking_portal_title", MaxVillagerCount);
					RemoveExcessVillagersInPortal();
				}
				if (num == cardCount && !GameCanvas.instance.ModalIsOpen)
				{
					MyGameCard.CancelTimer(GetActionId("TakePortal"));
					GameCanvas.instance.OneVillagerNeedsToStayPrompt("label_taking_portal_title");
					RemoveLastVillagerInPortal();
				}
				else
				{
					MyGameCard.StartTimer(TravelTime, base.TakePortal, SokLoc.Translate("card_stable_portal_status"), GetActionId("TakePortal"));
				}
			}
			else
			{
				MyGameCard.CancelTimer(GetActionId("TakePortal"));
			}
		}
		base.UpdateCard();
	}
}
