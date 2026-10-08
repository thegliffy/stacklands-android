using UnityEngine;

public class StatusEffect_NoSewer : StatusEffect
{
	protected override string TermId => "no_sewer";

	public override bool FadeInNonDefaultView => false;

	public override Sprite Sprite => SpriteManager.instance.NoSewerEffect;
}
