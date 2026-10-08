using UnityEngine;

public class StatusEffect_NoEducatedWorkers : StatusEffect
{
	protected override string TermId => "no_educated_workers";

	public override Sprite Sprite => SpriteManager.instance.NoEducatedWorkersEffect;

	public override bool FadeInNonDefaultView => false;
}
