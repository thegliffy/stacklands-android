using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Demand", menuName = "ScriptableObjects/Demand", order = 1)]
public class Demand : ScriptableObject
{
	public string DemandId;

	public int Duration;

	public DemandDifficulty Difficulty;

	public List<GreedAnimationState> QuestStartAnimationStates;

	public List<GreedAnimationState> QuestSuccessAnimationStates;

	public List<GreedAnimationState> QuestFailedAnimationStates;

	[Card]
	public string CardToGet;

	public int Amount;

	public bool ShouldDestroyOnComplete = true;

	[HideInInspector]
	public List<CardAmountPair> SuccessCards;

	[HideInInspector]
	public List<CardAmountPair> FailedCards;

	public List<string> BlueprintIds;

	public bool IsFinalDemand;

	public string GetStartTerm()
	{
		return DemandManager.instance.StartDemandLocTerms[Random.Range(0, DemandManager.instance.StartDemandLocTerms.Count - 1)];
	}

	public string GetSuccessTerm()
	{
		return DemandManager.instance.SuccessDemandLocTerms[Random.Range(0, DemandManager.instance.SuccessDemandLocTerms.Count - 1)];
	}

	public string GetFailedTerm()
	{
		return DemandManager.instance.FailedDemandLocTerms[Random.Range(0, DemandManager.instance.FailedDemandLocTerms.Count - 1)];
	}
}
