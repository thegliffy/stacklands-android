public class Tornado : EventCard
{
	protected override void ExecuteEvent()
	{
		WorldManager.instance.QueueCutscene(CitiesCutscenes.CitiesTornado());
		EventIsActive = true;
		base.ExecuteEvent();
	}
}
