public class FinancialCrisis : EventCard
{
	protected override void ExecuteEvent()
	{
		MyGameCard.StartTimer(WorldManager.instance.MonthTime, StopEvent, SokLoc.Translate("label_uh_oh"), GetActionId("StopEvent"));
		EventIsActive = true;
		WorldManager.instance.QueueCutscene(CitiesCutscenes.CitiesFinancialCrisis());
	}

	[TimedAction("stop_event")]
	public void StopEvent()
	{
		base.EndEvent();
	}

	protected override void EndEvent()
	{
		WorldManager.instance.QueueCutscene(CitiesCutscenes.CitiesStopDisaster());
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
