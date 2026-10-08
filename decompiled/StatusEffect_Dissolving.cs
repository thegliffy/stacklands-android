using UnityEngine;

public class StatusEffect_Dissolving : StatusEffect
{
	protected override string TermId => "dissolving";

	public override Sprite Sprite => SpriteManager.instance.DissolvingEffect;

	public override void Update()
	{
		float num = StatusTimer / WorldManager.instance.MonthTime;
		if (base.ParentCard is DissolvingResource dissolvingResource)
		{
			num *= dissolvingResource.DissolvingTimeMultiplier;
		}
		FillAmount = (num = 1f - num * 5f);
		if (num <= 0f)
		{
			if (base.ParentCard is DissolvingResource dissolvingResource2)
			{
				dissolvingResource2.Dissolve();
			}
			else
			{
				base.ParentCard.MyGameCard.DestroyCard(spawnSmoke: true);
			}
		}
		base.Update();
	}
}
