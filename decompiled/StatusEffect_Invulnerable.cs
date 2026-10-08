using UnityEngine;

public class StatusEffect_Invulnerable : StatusEffect
{
	[ExtraData("invulnerable_timer")]
	public float InvulnerableTimer;

	private const float invulnerableTime = 5f;

	protected override string TermId => "invulnerable";

	public override Sprite Sprite => SpriteManager.instance.InvulnerableEffect;

	public override void Update()
	{
		FillAmount = 1f - InvulnerableTimer / 5f;
		InvulnerableTimer += Time.deltaTime * WorldManager.instance.TimeScale;
		if (InvulnerableTimer >= 5f)
		{
			InvulnerableTimer = 0f;
			base.ParentCard.RemoveStatusEffect(this);
		}
		base.Update();
	}
}
