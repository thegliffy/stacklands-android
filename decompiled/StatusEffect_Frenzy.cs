using UnityEngine;

public class StatusEffect_Frenzy : StatusEffect
{
	[ExtraData("frenzy_timer")]
	public float FrenzyTimer;

	protected override string TermId => "frenzy";

	public override Sprite Sprite => SpriteManager.instance.FrenzyEffect;

	public override void Update()
	{
		FillAmount = 1f - FrenzyTimer / 10f;
		FrenzyTimer += Time.deltaTime * WorldManager.instance.TimeScale;
		if (FrenzyTimer >= 10f)
		{
			FrenzyTimer = 0f;
			base.ParentCard.RemoveStatusEffect(this);
		}
		base.Update();
	}
}
