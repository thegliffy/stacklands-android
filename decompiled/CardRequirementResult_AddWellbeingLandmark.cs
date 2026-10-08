using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class CardRequirementResult_AddWellbeingLandmark : CardRequirementResult
{
	public int Amount;

	public override IEnumerator EndOfCutscenePerform(GameCard card)
	{
		WorldManager.instance.CurrentRunVariables.BuiltLandmarks.Add(card.CardData.Id);
		WorldManager.instance.CreateWellbeingPlus(card.Position);
		AudioManager.me.PlaySound2D(AudioManager.me.LandmarkBuild, 1f, 0.3f);
		WorldManager.instance.QueueCutsceneIfNotPlayed("first_landmark");
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
		return $"<color=#{ColorUtility.ToHtmlStringRGB(ColorManager.instance.FloatingTextColorSuccess)}><nobr>{CitiesManager.GetAmountPrefix(Amount)}{Amount * multiplier} {Icons.Wellbeing}</nobr></color>";
	}

	public override string RequirementDescriptionPositive(int multiplier, GameCard card)
	{
		return $"<color=#{ColorUtility.ToHtmlStringRGB(ColorManager.instance.FloatingTextColorSuccess)}><nobr>{CitiesManager.GetAmountPrefix(Amount)}{Amount * multiplier}{Icons.Wellbeing}</nobr></color>";
	}
}
