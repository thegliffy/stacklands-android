public class Kraken : Enemy
{
	public override void Die()
	{
		WorldManager.instance.CurrentRunVariables.FinishedKraken = true;
		base.Die();
	}
}
