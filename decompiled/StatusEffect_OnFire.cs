using UnityEngine;

public class StatusEffect_OnFire : StatusEffect
{
	protected override string TermId => "on_fire";

	public override bool FadeInNonDefaultView => false;

	public override Sprite Sprite => SpriteManager.instance.OnFireEffect;
}
