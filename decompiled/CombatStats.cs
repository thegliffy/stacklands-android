using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CombatStats
{
	public int MaxHealth;

	public float AttackSpeed = 3.5f;

	public float HitChance = 0.5f;

	public int AttackDamage = 1;

	public int Defence = 1;

	public int AttackSpeedIncrement;

	public int HitChanceIncrement;

	public int AttackDamageIncrement;

	public int DefenceIncrement;

	public List<SpecialHit> SpecialHits;

	private static Dictionary<Type, int> enumLengths = new Dictionary<Type, int>();

	public float CombatLevel
	{
		get
		{
			float num = 0f;
			num += CalculateAverageAttackDamagePerSecond() * 5f;
			num += (float)MaxHealth * 0.5f;
			num += (float)(Defence + DefenceIncrement) * 2f;
			if (SpecialHits.Count > 0)
			{
				float num2 = 0f;
				foreach (SpecialHit specialHit in SpecialHits)
				{
					bool flag = specialHit.Target == SpecialHitTarget.Self || specialHit.Target == SpecialHitTarget.RandomFriendly || specialHit.Target == SpecialHitTarget.AllFriendly;
					num2 = ((!specialHit.IsDebuff()) ? ((!flag) ? (num2 - specialHit.Chance / 10f) : (num2 + specialHit.Chance / 10f)) : ((!flag) ? (num2 + specialHit.Chance / 10f) : (num2 - specialHit.Chance / 10f)));
				}
				num += num2 * 2f;
			}
			return Mathf.Max(1f, num);
		}
	}

	public float ItemLevel
	{
		get
		{
			float num = 0f;
			num += (float)IncrementAttackDefence(2, AttackDamageIncrement) / IncrementAttackSpeed(2.9f, AttackSpeedIncrement) * IncrementHitChance(0.68f, HitChanceIncrement) * 5f;
			num += (float)(15 + MaxHealth) * 0.5f;
			num += (float)IncrementAttackDefence(2, DefenceIncrement) * 2f;
			if (SpecialHits.Count > 0)
			{
				float num2 = 0f;
				foreach (SpecialHit specialHit in SpecialHits)
				{
					bool flag = specialHit.Target == SpecialHitTarget.Self || specialHit.Target == SpecialHitTarget.RandomFriendly || specialHit.Target == SpecialHitTarget.AllFriendly;
					num2 = ((!specialHit.IsDebuff()) ? ((!flag) ? (num2 - specialHit.Chance / 10f) : (num2 + specialHit.Chance / 10f)) : ((!flag) ? (num2 + specialHit.Chance / 10f) : (num2 - specialHit.Chance / 10f)));
				}
				num += num2 * 2f;
			}
			num -= 14f;
			return Mathf.Max(1f, num);
		}
	}

	public void InitStats(CombatStats stats)
	{
		HitChance = stats.HitChance;
		AttackSpeed = stats.AttackSpeed;
		SpecialHits = new List<SpecialHit>(stats.SpecialHits);
		MaxHealth = stats.MaxHealth;
		AttackDamage = stats.AttackDamage;
		Defence = stats.Defence;
	}

	public void AddStats(CombatStats equipment)
	{
		HitChance = IncrementHitChance(HitChance, equipment.HitChanceIncrement);
		AttackSpeed = IncrementAttackSpeed(AttackSpeed, equipment.AttackSpeedIncrement);
		SpecialHits = AddSpecialHits(equipment.SpecialHits);
		MaxHealth += equipment.MaxHealth;
		AttackDamage = IncrementAttackDefence(AttackDamage, equipment.AttackDamageIncrement);
		Defence = IncrementAttackDefence(Defence, equipment.DefenceIncrement);
	}

	public string SummarizeSpecialHits()
	{
		string text = "";
		for (int i = 0; i < SpecialHits.Count; i++)
		{
			SpecialHit specialHit = SpecialHits[i];
			text += specialHit.GetText();
			if (i < SpecialHits.Count - 1)
			{
				text += "\n";
			}
		}
		return text;
	}

	public List<SpecialHit> AddSpecialHits(List<SpecialHit> specialHits)
	{
		List<SpecialHit> list = new List<SpecialHit>();
		foreach (SpecialHit hit2 in specialHits)
		{
			SpecialHit specialHit = SpecialHits.Find((SpecialHit x) => x.HitType == hit2.HitType && x.Target == hit2.Target);
			SpecialHit specialHit2 = new SpecialHit();
			specialHit2.HitType = hit2.HitType;
			specialHit2.Target = hit2.Target;
			specialHit2.Chance = hit2.Chance;
			if (specialHit != null)
			{
				specialHit2.Chance += specialHit.Chance;
			}
			list.Add(specialHit2);
		}
		foreach (SpecialHit hit in SpecialHits)
		{
			if (list.FindIndex((SpecialHit x) => x.HitType == hit.HitType && x.Target == hit.Target) == -1)
			{
				list.Add(hit);
			}
		}
		return list;
	}

	private float CalculateAverageAttackDamagePerSecond()
	{
		return (float)AttackDamage / AttackSpeed * HitChance;
	}

	public string GetHitChanceTranslation()
	{
		return GetHitChanceEnum(HitChance).TranslateEnum();
	}

	public string GetAttackSpeedTranslation()
	{
		return GetAttackTimeEnum(AttackSpeed).TranslateEnum();
	}

	public string GetAttackDamageTranslation()
	{
		return GetAttackDamageEnum(AttackDamage).TranslateEnum();
	}

	public string GetDefenceTranslation()
	{
		return GetDefenceEnum(Defence).TranslateEnum();
	}

	public static HitChance GetHitChanceEnum(float hitChange)
	{
		if (EqualsFloat(hitChange, 0.5f))
		{
			return global::HitChance.VerySmall;
		}
		if (EqualsFloat(hitChange, 0.59f))
		{
			return global::HitChance.Small;
		}
		if (EqualsFloat(hitChange, 0.68f))
		{
			return global::HitChance.Normal;
		}
		if (EqualsFloat(hitChange, 0.77f))
		{
			return global::HitChance.High;
		}
		if (EqualsFloat(hitChange, 0.86f))
		{
			return global::HitChance.VeryHigh;
		}
		if (EqualsFloat(hitChange, 0.95f))
		{
			return global::HitChance.ExtremelyHigh;
		}
		return global::HitChance.Normal;
	}

	public static AttackSpeed GetAttackTimeEnum(float attackTime)
	{
		if (EqualsFloat(attackTime, 3.5f))
		{
			return global::AttackSpeed.VerySlow;
		}
		if (EqualsFloat(attackTime, 2.9f))
		{
			return global::AttackSpeed.Slow;
		}
		if (EqualsFloat(attackTime, 2.3f))
		{
			return global::AttackSpeed.Normal;
		}
		if (EqualsFloat(attackTime, 1.7f))
		{
			return global::AttackSpeed.Fast;
		}
		if (EqualsFloat(attackTime, 1.1f))
		{
			return global::AttackSpeed.VeryFast;
		}
		if (EqualsFloat(attackTime, 0.5f))
		{
			return global::AttackSpeed.ExtremelyFast;
		}
		return global::AttackSpeed.Normal;
	}

	public static AttackDamage GetAttackDamageEnum(int attackDamage)
	{
		if (attackDamage == 1)
		{
			return global::AttackDamage.VeryWeak;
		}
		if (attackDamage == 2)
		{
			return global::AttackDamage.Weak;
		}
		if (attackDamage == 3)
		{
			return global::AttackDamage.Normal;
		}
		if (attackDamage == 4)
		{
			return global::AttackDamage.Strong;
		}
		if (attackDamage == 5)
		{
			return global::AttackDamage.VeryStrong;
		}
		if (attackDamage >= 6)
		{
			return global::AttackDamage.ExtremelyStrong;
		}
		return global::AttackDamage.Normal;
	}

	public static Defence GetDefenceEnum(int defence)
	{
		if (defence == 1)
		{
			return global::Defence.VeryWeak;
		}
		if (defence == 2)
		{
			return global::Defence.Weak;
		}
		if (defence == 3)
		{
			return global::Defence.Normal;
		}
		if (defence == 4)
		{
			return global::Defence.Strong;
		}
		if (defence == 5)
		{
			return global::Defence.VeryStrong;
		}
		if (defence >= 6)
		{
			return global::Defence.ExtremelyStrong;
		}
		return global::Defence.Normal;
	}

	private static bool EqualsFloat(float a, float b)
	{
		return (double)Mathf.Abs(a - b) < 0.01;
	}

	private static int GetEnumLength<T>()
	{
		if (!enumLengths.ContainsKey(typeof(T)))
		{
			enumLengths[typeof(T)] = Enum.GetNames(typeof(T)).Length;
		}
		return enumLengths[typeof(T)];
	}

	public static float IncrementAttackSpeed(float current, int increment)
	{
		return Mathf.Clamp(current - (float)increment * 0.6f, 0.5f, 3.5f);
	}

	public static float IncrementHitChance(float current, int increment)
	{
		return Mathf.Clamp(current + (float)increment * 0.09f, 0.5f, 0.95f);
	}

	public static int IncrementAttackDefence(int current, int increment)
	{
		return current + increment;
	}
}
