public class BlueprintHappiness : Blueprint
{
	public override bool CanCurrentlyBeMade => WorldManager.instance.CurseIsActive(CurseType.Happiness);
}
