using UnityEngine;

public class StatusEffect_Homeless : StatusEffect
{
	protected override string TermId => "homeless";

	public override Sprite Sprite => SpriteManager.instance.HomelessEffect;
}
