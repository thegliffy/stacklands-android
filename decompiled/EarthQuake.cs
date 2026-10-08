public class EarthQuake : EventCard
{
	protected override void ExecuteEvent()
	{
		EventIsActive = true;
		WorldManager.instance.QueueCutscene(CitiesCutscenes.CitiesEarthQuake(MyGameCard));
	}
}
