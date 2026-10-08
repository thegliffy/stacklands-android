using UnityEngine;

public class StatusEffect_Radar : StatusEffect
{
	protected override string TermId => "radar";

	public override Sprite Sprite => SpriteManager.instance.RadarEffect;

	public override bool FadeInNonDefaultView => false;

	public override string Description => SokLoc.Translate("statuseffect_radar_description", LocParam.Create("amount", (CitiesManager.instance.NextConflictMonth - 1).ToString()));

	public override void Update()
	{
		bool flag = StatusTimer > 1f;
		FillAmount = (flag ? 1f : 0f);
		if (StatusTimer > 2f)
		{
			StatusTimer = 0f;
		}
		base.Update();
	}
}
