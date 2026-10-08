using System;

[Serializable]
public class SpecialHit
{
	public float Chance = 1f;

	public SpecialHitType HitType;

	public SpecialHitTarget Target;

	public string GetText()
	{
		string value = SokLoc.Translate("target_" + Target.ToString().ToLower());
		return SokLoc.Translate("specialhit_" + HitType.ToString().ToLower() + "_long", LocParam.Create("chance", Chance.ToString()), LocParam.Create("target", value));
	}

	public bool IsDebuff()
	{
		if (HitType == SpecialHitType.Poison || HitType == SpecialHitType.Stun || HitType == SpecialHitType.LifeSteal || HitType == SpecialHitType.Bleeding || HitType == SpecialHitType.Damage || HitType == SpecialHitType.Crit || HitType == SpecialHitType.Sick || HitType == SpecialHitType.Anxious)
		{
			return true;
		}
		return false;
	}
}
