using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class CardRequirementResult_AddWellbeing : CardRequirementResult
{
	public int Amount;

	public bool IsNegative;

	public override IEnumerator EndOfCutscenePerform(GameCard card)
	{
		return null;
	}

	public override RequirementType GetRequirementType()
	{
		return RequirementType.WellBeing;
	}

	public override IEnumerator Perform(GameCard card)
	{
		CitiesManager.instance.AddWellbeing(Amount);
		card.CardData.UpdateRequirementResultsInStack(RequirementType.WellBeing, Amount, card);
		return null;
	}

	public override string RequirementDescriptionNegative(int multiplier, GameCard card)
	{
		if (IsNegative)
		{
			return $"<color=#{ColorUtility.ToHtmlStringRGB(ColorManager.instance.FloatingTextColorFailed)}><nobr>{CitiesManager.GetAmountPrefix(Amount)}{Amount * multiplier}{Icons.Wellbeing}</nobr></color>";
		}
		return $"<color=#{ColorUtility.ToHtmlStringRGB(ColorManager.instance.FloatingTextColorSuccess)}><nobr>{CitiesManager.GetAmountPrefix(Amount)}{Amount * multiplier}{Icons.Wellbeing}</nobr></color>";
	}

	public override string RequirementDescriptionPositive(int multiplier, GameCard card)
	{
		if (IsNegative)
		{
			return $"<color=#{ColorUtility.ToHtmlStringRGB(ColorManager.instance.FloatingTextColorFailed)}><nobr>{CitiesManager.GetAmountPrefix(Amount)}{Amount * multiplier}{Icons.Wellbeing}</nobr></color>";
		}
		return $"<color=#{ColorUtility.ToHtmlStringRGB(ColorManager.instance.FloatingTextColorSuccess)}><nobr>{CitiesManager.GetAmountPrefix(Amount)}{Amount * multiplier}{Icons.Wellbeing}</nobr></color>";
	}
}
