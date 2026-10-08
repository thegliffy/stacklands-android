public class PackSale : EventCard
{
	protected override void ExecuteEvent()
	{
		MyGameCard.StartTimer(WorldManager.instance.MonthTime / 2f, StopEvent, SokLoc.Translate("label_nice"), GetActionId("StopEvent"));
		WorldManager.instance.QueueCutscene("cities_pack_sale");
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
