using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class CardRequirementResult_HealCombatable : CardRequirementResult
{
	public bool IsNegative;

	public int HealthPerMonth;

	public override IEnumerator EndOfCutscenePerform(GameCard card)
	{
		return null;
	}

	public override RequirementType GetRequirementType()
	{
		return RequirementType.Health;
	}

	public override IEnumerator Perform(GameCard card)
	{
		if (card.CardData is CitiesCombatable citiesCombatable)
		{
			citiesCombatable.HealthPoints = Mathf.Min(citiesCombatable.HealthPoints + HealthPerMonth, citiesCombatable.ProcessedCombatStats.MaxHealth);
		}
		return null;
	}

	public override string RequirementDescriptionNegative(int multiplier, GameCard card)
	{
		return "";
	}

	public override string RequirementDescriptionPositive(int multiplier, GameCard card)
	{
		return "";
	}
}
