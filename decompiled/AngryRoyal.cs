public class AngryRoyal : Enemy
{
	public override void Die()
	{
		WorldManager.instance.QueueCutscene(GreedCutscenes.KillRoyalLiftCurse());
		base.Die();
	}

	public void DieInCutscene()
	{
		WorldManager.instance.CreateCard(base.transform.position, "royal_crown");
		WorldManager.instance.CreateSmoke(base.transform.position);
		RemoveAllStatusEffects();
		WorldManager.instance.ChangeToCard(MyGameCard, "corpse");
	}
}
