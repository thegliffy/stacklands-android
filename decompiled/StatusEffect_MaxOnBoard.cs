using UnityEngine;

public class StatusEffect_MaxOnBoard : StatusEffect
{
	protected override string TermId => "max_on_board";

	public override Sprite Sprite => SpriteManager.instance.MaxReachedEffect;
}
