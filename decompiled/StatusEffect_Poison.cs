using UnityEngine;

public class StatusEffect_Poison : StatusEffect
{
	[ExtraData("damage_timer")]
	public float DamageTimer;

	private float timeToDamage = 60f;

	[ExtraData("poison_count")]
	public int PoisonCount;

	protected override string TermId => "poison";

	public override Sprite Sprite => SpriteManager.instance.PoisonEffect;

	public override void Update()
	{
		if (base.ParentCard is Enemy)
		{
			timeToDamage = 30f;
		}
		else
		{
			timeToDamage = 60f;
		}
		FillAmount = 1f - DamageTimer / timeToDamage;
		DamageTimer += Time.deltaTime * WorldManager.instance.TimeScale;
		if (DamageTimer >= timeToDamage)
		{
			DamageTimer = 0f;
			Combatable combatable = base.ParentCard as Combatable;
			if (combatable != null)
			{
				PoisonCount++;
				combatable.Damage(3);
				combatable.CreateHitText("3", PrefabManager.instance.PoisonHitText);
				AudioManager.me.PlaySound2D(AudioManager.me.Poison, Random.Range(0.8f, 1.2f), 0.2f);
				if (base.ParentCard is Enemy && PoisonCount >= 3)
				{
					base.ParentCard.RemoveStatusEffect(this);
				}
			}
		}
		base.Update();
	}
}
