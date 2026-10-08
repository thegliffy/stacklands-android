public class Drought : EventCard
{
	protected override void ExecuteEvent()
	{
		EventIsActive = true;
		WorldManager.instance.QueueCutscene(CitiesCutscenes.CitiesDrought(MyGameCard));
	}
}
