using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class CardRequirementResult_UsePollution : CardRequirementResult
{
	public int PollutionPerWellbeing;

	public int WellbeingAmount
	{
		get
		{
			Pollution card = WorldManager.instance.GetCard<Pollution>();
			if (card != null && card.PollutionAmount > 0)
			{
				return -Mathf.RoundToInt(card.PollutionAmount / PollutionPerWellbeing);
			}
			return 0;
		}
	}

	public override IEnumerator EndOfCutscenePerform(GameCard card)
	{
		return null;
	}

	public override RequirementType GetRequirementType()
	{
		return RequirementType.Pollution;
	}

	public override IEnumerator Perform(GameCard card)
	{
		if (card.CardData is Pollution pollution)
		{
			int num = -Mathf.RoundToInt(pollution.PollutionAmount / PollutionPerWellbeing);
			CitiesManager.instance.AddWellbeing(num);
			card.CardData.UpdateRequirementResultsInStack(RequirementType.Pollution, num, card);
		}
		return null;
	}

	public override string RequirementDescriptionNegative(int multiplier, GameCard card)
	{
		return $"<color=#{ColorUtility.ToHtmlStringRGB(ColorManager.instance.FloatingTextColorSuccess)}><nobr>{-1 * multiplier}{Icons.Wellbeing}</nobr></color>";
	}

	public override string RequirementDescriptionPositive(int multiplier, GameCard card)
	{
		return $"<color=#{ColorUtility.ToHtmlStringRGB(ColorManager.instance.FloatingTextColorFailed)}><nobr>{-1 * multiplier}{Icons.Wellbeing}</nobr></color>";
	}
}
