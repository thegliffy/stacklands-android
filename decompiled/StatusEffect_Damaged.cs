using UnityEngine;

public class StatusEffect_Damaged : StatusEffect
{
	protected override string TermId => "damaged";

	public override bool FadeInNonDefaultView => false;

	public override Sprite Sprite => SpriteManager.instance.DamagedEffect;
}
