using UnityEngine;

public class StatusEffect_CardOff : StatusEffect
{
	protected override string TermId => "card_off";

	public override Sprite Sprite => SpriteManager.instance.CardOffEffect;
}
