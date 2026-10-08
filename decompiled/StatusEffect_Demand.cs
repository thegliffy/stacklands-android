using UnityEngine;

public class StatusEffect_Demand : StatusEffect
{
	protected override string TermId => "demand";

	public override Sprite Sprite => SpriteManager.instance.DemandEffect;
}
