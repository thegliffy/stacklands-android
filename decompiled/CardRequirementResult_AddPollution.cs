using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class CardRequirementResult_AddPollution : CardRequirementResult
{
	public int Amount;

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
		Pollution obj = WorldManager.instance.CreateCard(card.Position, "pollution") as Pollution;
		obj.PollutionAmount = Amount;
		obj.MyGameCard.SendIt();
		card.CardData.UpdateRequirementResultsInStack(RequirementType.Pollution, -Amount, card);
		return null;
	}

	public override string RequirementDescriptionNegative(int multiplier, GameCard card)
	{
		return $"<color=#{ColorUtility.ToHtmlStringRGB(ColorManager.instance.FloatingTextColorFailed)}><nobr>{CitiesManager.GetAmountPrefix(Amount)}{Amount * multiplier}{Icons.Pollution}</nobr></color>";
	}

	public override string RequirementDescriptionPositive(int multiplier, GameCard card)
	{
		return $"<color=#{ColorUtility.ToHtmlStringRGB(ColorManager.instance.FloatingTextColorFailed)}><nobr>{CitiesManager.GetAmountPrefix(Amount)}{Amount * multiplier}{Icons.Pollution}</nobr></color>";
	}
}
