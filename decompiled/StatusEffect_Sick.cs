using UnityEngine;

public class StatusEffect_Sick : StatusEffect
{
	private const int damage = 2;

	private const float damageTime = 30f;

	protected override string TermId => "sick";

	public override Sprite Sprite => SpriteManager.instance.SickEffect;

	public override string Description => SokLoc.Translate("statuseffect_sick_description", LocParam.Create("damage", 2.ToString()), LocParam.Create("time", GameCanvas.FormatTimeShort(30f)));

	public override void Update()
	{
		FillAmount = 1f - StatusTimer / 30f;
		if (StatusTimer >= 30f)
		{
			StatusTimer = 0f;
			if (base.ParentCard is Combatable combatable)
			{
				combatable.Damage(2);
				combatable.CreateHitText(2.ToString(), PrefabManager.instance.SickHitText);
				AudioManager.me.PlaySound2D(AudioManager.me.Poison, Random.Range(0.8f, 1.2f), 0.2f);
			}
		}
		base.Update();
	}
}
