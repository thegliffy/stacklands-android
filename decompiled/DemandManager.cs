using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DemandManager : MonoBehaviour
{
	public static DemandManager instance;

	public bool CanReceiveDemand = true;

	public List<AudioClip> StartDemandSound;

	public List<AudioClip> FinishDemandSound;

	public List<AudioClip> FailedDemandSound;

	[HideInInspector]
	public List<Demand> AllDemands = new List<Demand>();

	public List<string> StartDemandLocTerms;

	public List<string> FailedDemandLocTerms;

	public List<string> SuccessDemandLocTerms;

	private void Awake()
	{
		instance = this;
		AllDemands = WorldManager.instance.GameDataLoader.Demands;
		if (AllDemands.Count < 1)
		{
			Debug.LogError("No Demands were loaded");
		}
	}

	public Demand GetCurrentDemand()
	{
		return WorldManager.instance.CurrentRunVariables.ActiveDemand?.Demand;
	}

	public IEnumerator CheckDemands(int month)
	{
		if (!CanReceiveDemand)
		{
			yield break;
		}
		DemandEvent activeDemand = WorldManager.instance.CurrentRunVariables.ActiveDemand;
		Demand currentDemand = GetCurrentDemand();
		if (currentDemand != null)
		{
			if (currentDemand.IsFinalDemand && (bool)WorldManager.instance.GetCard<DragonEgg>())
			{
				yield return GreedCutscenes.FinalDemandEndSuccess(shouldStop: false);
			}
			else if (activeDemand.MonthCompleted == month)
			{
				yield return FinishDemand(activeDemand);
			}
		}
		else
		{
			Demand demandToStart = GetDemandToStart(month);
			if (demandToStart != null)
			{
				yield return StartDemand(demandToStart);
			}
		}
	}

	public string GetRandomStartDescription(Demand demand)
	{
		return SokLoc.Translate(demand.GetStartTerm(), LocParam.Create("cardsToGet", $"{demand.Amount} x {WorldManager.instance.GameDataLoader.GetCardFromId(demand.CardToGet).Name}"), LocParam.Create("month", demand.Duration.ToString()), LocParam.Create("monthFinished", (WorldManager.instance.CurrentMonth + demand.Duration - 1).ToString()));
	}

	public string GetDemandStartDescription(Demand demand, DemandEvent demandEvent = null)
	{
		return SokLoc.Translate("label_" + demand.DemandId + "_text", LocParam.Create("amount", demand.Amount.ToString()), LocParam.Create("monthFinished", ((demandEvent?.MonthStarted ?? WorldManager.instance.CurrentMonth) + demand.Duration - 1).ToString()));
	}

	public string GetRandomSuccessDescription(Demand demand)
	{
		return SokLoc.Translate(demand.GetSuccessTerm(), LocParam.Create("cardsToGet", $"{demand.Amount} x {WorldManager.instance.GameDataLoader.GetCardFromId(demand.CardToGet).Name}"), LocParam.Create("month", demand.Duration.ToString()));
	}

	public string GetRandomFailedDescription(Demand demand)
	{
		return SokLoc.Translate(demand.GetFailedTerm(), LocParam.Create("cardsToGet", $"{demand.Amount} x {WorldManager.instance.GameDataLoader.GetCardFromId(demand.CardToGet).Name}"), LocParam.Create("month", demand.Duration.ToString()));
	}

	public Demand GetDemandToStart(int month)
	{
		int num = month - WorldManager.instance.CurrentRunVariables.LastDemandMonth;
		if (WorldManager.instance.CurrentRunVariables.PreviousDemandEvents.Count == 0)
		{
			return GetPossibleDemands().FirstOrDefault();
		}
		if (num == 1)
		{
			return GetPossibleDemands().FirstOrDefault();
		}
		return null;
	}

	public List<Demand> GetPossibleDemands()
	{
		return AllDemands.Where((Demand x) => WorldManager.instance.CurrentRunVariables.PreviousDemandEvents.FindIndex((DemandEvent e) => e.DemandId == x.DemandId) == -1).OrderBy(delegate(Demand demand)
		{
			if (demand.IsFinalDemand)
			{
				return 5;
			}
			if (demand.Difficulty == DemandDifficulty.easy)
			{
				return 1;
			}
			if (demand.Difficulty == DemandDifficulty.medium)
			{
				return 2;
			}
			return (demand.Difficulty == DemandDifficulty.hard) ? 3 : 4;
		}).ToList();
	}

	public IEnumerator StartDemand(Demand demand)
	{
		AudioManager.me.PlaySound2D(StartDemandSound, 0.9f, 0.3f);
		QuestManager.instance.SpecialActionComplete("demand_start");
		if (demand.IsFinalDemand)
		{
			yield return GreedCutscenes.FinalDemandStart(AllDemands.Find((Demand x) => x.IsFinalDemand));
		}
		else
		{
			yield return GreedCutscenes.StartDemand(demand);
		}
		if (WorldManager.instance.CurrentRunVariables.PreviousDemandEvents.Count == 2)
		{
			yield return GreedCutscenes.NewVillager();
		}
		if (WorldManager.instance.CurrentRunVariables.PreviousDemandEvents.Count == 5)
		{
			WorldManager.instance.QueueCutsceneIfNotPlayed("greed_middle");
		}
		if (WorldManager.instance.CurrentRunVariables.PreviousDemandEvents.Count == 9)
		{
			WorldManager.instance.QueueCutsceneIfNotPlayed("greed_end");
		}
	}

	public void QuestStarted(Demand demand)
	{
		WorldManager.instance.CurrentRunVariables.ActiveDemand = new DemandEvent(demand.DemandId, WorldManager.instance.CurrentMonth, demand.Duration, WorldManager.instance.CurrentBoard.Id);
	}

	public IEnumerator FinishDemand(DemandEvent demandEvent)
	{
		WorldManager.instance.CutsceneTitle = "";
		WorldManager.instance.CutsceneText = "";
		Demand demandById = GetDemandById(demandEvent.DemandId);
		demandEvent.Completed = true;
		AudioManager.me.PlaySound2D(FinishDemandSound, 0.9f, 0.3f);
		yield return WorldManager.instance.FinishDemand(demandById, demandEvent);
	}

	public void DemandFinishedSuccess(Demand demand)
	{
		foreach (CardAmountPair successCard in demand.SuccessCards)
		{
			for (int i = 0; i < successCard.Amount; i++)
			{
				WorldManager.instance.CreateCard(base.transform.position, successCard.CardId, faceUp: false, checkAddToStack: false).MyGameCard.SendIt();
			}
		}
	}

	public List<Combatable> SpawnEnemies()
	{
		float maxStrength = GetTimesDemandFailed() * 20;
		Combatable item = WorldManager.instance.GetCardPrefab("royal_guard") as Combatable;
		Combatable item2 = WorldManager.instance.GetCardPrefab("royal_archer") as Combatable;
		Combatable item3 = WorldManager.instance.GetCardPrefab("royal_mage") as Combatable;
		List<CardIdWithEquipment> enemiesToSpawn = SpawnHelper.GetEnemiesToSpawn(new List<Combatable> { item, item2, item3 }, maxStrength);
		List<Combatable> list = new List<Combatable>();
		foreach (CardIdWithEquipment item4 in enemiesToSpawn)
		{
			Vector3 position = ((list.Count == 0) ? WorldManager.instance.GetRandomSpawnPosition() : list[0].Position);
			Combatable combatable = WorldManager.instance.CreateCard(position, item4, faceUp: false, checkAddToStack: false) as Combatable;
			WorldManager.instance.CreateSmoke(combatable.Position);
			combatable.HealthPoints = combatable.ProcessedCombatStats.MaxHealth;
			combatable.MyGameCard.SendIt();
			list.Add(combatable);
		}
		return list;
	}

	public Demand GetDemandById(string demandId)
	{
		return AllDemands.Find((Demand x) => x.DemandId == demandId);
	}

	public int GetTimesDemandFailed()
	{
		return Mathf.Max(1, WorldManager.instance.CurrentRunVariables.PreviousDemandEvents.Count((DemandEvent x) => !x.Successful));
	}

	public void ResetDemands()
	{
		WorldManager.instance.CurrentRunVariables.PreviousDemandEvents.Clear();
		WorldManager.instance.CurrentRunVariables.ActiveDemand = null;
		CanReceiveDemand = true;
	}
}
