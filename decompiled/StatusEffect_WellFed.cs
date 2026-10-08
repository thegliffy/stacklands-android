using UnityEngine;

public class StatusEffect_WellFed : StatusEffect
{
	protected override string TermId => "wellfed";

	public override Sprite Sprite => SpriteManager.instance.WellFedEffect;

	public override void Update()
	{
		FillAmount = 1f - StatusTimer / WorldManager.instance.MonthTime;
		if (StatusTimer >= WorldManager.instance.MonthTime)
		{
			base.ParentCard.RemoveStatusEffect(this);
		}
		base.Update();
	}
}
