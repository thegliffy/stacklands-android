public class Festival : EventCard
{
	protected override void ExecuteEvent()
	{
		MyGameCard.StartTimer(5f, StopEvent, SokLoc.Translate(EventText), GetActionId("StopEvent"));
		WorldManager.instance.QueueCutscene("cities_festival");
		CardData cardData = WorldManager.instance.CreateCard(base.Position, "merch", faceUp: true, checkAddToStack: false);
		WorldManager.instance.CreateWellbeingPlus(base.Position);
		cardData.MyGameCard.SendIt();
		EventIsActive = true;
	}

	[TimedAction("stop_event")]
	public void StopEvent()
	{
		base.EndEvent();
	}

	protected override void EndEvent()
	{
		base.EndEvent();
	}

	public override void UpdateCardText()
	{
		if (MyGameCard != null && MyGameCard.TimerRunning && MyGameCard.TimerActionId == GetActionId("StopEvent"))
		{
			descriptionOverride = SokLoc.Translate(EventText);
		}
		base.UpdateCardText();
	}
}
