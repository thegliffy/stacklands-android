using UnityEngine;

public class StatusEffect_Drought : StatusEffect
{
	protected override string TermId => "drought";

	public override bool FadeInNonDefaultView => false;

	public override Sprite Sprite => SpriteManager.instance.DroughtEffect;
}
