public class WildFire : EventCard
{
	protected override void ExecuteEvent()
	{
		EventIsActive = true;
		WorldManager.instance.QueueCutscene(CitiesCutscenes.CitiesWildFire(MyGameCard));
	}
}
