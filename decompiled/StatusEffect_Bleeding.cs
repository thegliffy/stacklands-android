using UnityEngine;

public class StatusEffect_Bleeding : StatusEffect
{
	[ExtraData("damage_timer")]
	public float DamageTimer;

	protected override string TermId => "bleeding";

	public override Sprite Sprite => SpriteManager.instance.BleedingEffect;

	public override void Update()
	{
		FillAmount = 1f - StatusTimer / 10f;
		DamageTimer += Time.deltaTime * WorldManager.instance.TimeScale;
		if (DamageTimer >= 2f)
		{
			Combatable combatable = base.ParentCard as Combatable;
			if (combatable != null)
			{
				combatable.Damage(1);
				AudioManager.me.PlaySound2D(AudioManager.me.Bleed, Random.Range(0.8f, 1.2f), 0.2f);
				combatable.CreateHitText("1", PrefabManager.instance.BleedHitText);
			}
			DamageTimer = 0f;
		}
		if (StatusTimer >= 10f)
		{
			DamageTimer = 0f;
			StatusTimer = 0f;
			base.ParentCard.RemoveStatusEffect(this);
		}
		base.Update();
	}
}
