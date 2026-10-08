public class Crab : Animal
{
	public override void Die()
	{
		if (!WorldManager.instance.CurrentRunOptions.IsPeacefulMode)
		{
			WorldManager.instance.CurrentRunVariables.CrabsKilled++;
			if (WorldManager.instance.CurrentRunVariables.CrabsKilled % 3 == 0)
			{
				CardData mommaCrab = WorldManager.instance.CreateCard(base.transform.position, "momma_crab", faceUp: false, checkAddToStack: false);
				WorldManager.instance.QueueCutscene(Cutscenes.MommaCrab(mommaCrab));
			}
		}
		base.Die();
	}
}
