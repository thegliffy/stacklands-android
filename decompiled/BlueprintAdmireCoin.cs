public class BlueprintAdmireCoin : Blueprint
{
	public override bool CanCurrentlyBeMade => WorldManager.instance.CurseIsActive(CurseType.Happiness);
}
