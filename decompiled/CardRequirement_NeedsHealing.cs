using System;

[Serializable]
public class CardRequirement_NeedsHealing : CardRequirement
{
	public override string RequirementDescriptionNeed(int multiplier)
	{
		return SokLoc.Translate("label_requirement_needs_healing");
	}

	public override string RequirementDescriptionNeedNegative(int multiplier)
	{
		return SokLoc.Translate("label_requirement_needs_healing_negative");
	}

	public override bool Satisfied(GameCard card)
	{
		if (card.CardData is CitiesCombatable citiesCombatable && citiesCombatable.HealthPoints < citiesCombatable.ProcessedCombatStats.MaxHealth)
		{
			return true;
		}
		return false;
	}
}
