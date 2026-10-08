using UnityEngine;

public class StatusEffect_Depleted : StatusEffect
{
	protected override string TermId => "depleted";

	public override bool FadeInNonDefaultView => false;

	public override Sprite Sprite => SpriteManager.instance.BleedingEffect;
}
