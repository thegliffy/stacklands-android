using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
	public static QuestManager instance;

	public List<Quest> AllQuests = new List<Quest>();

	public List<Quest> CurrentQuests = new List<Quest>();

	private Dictionary<string, Quest> idToQuest = new Dictionary<string, Quest>();

	private List<Quest> sortedQuests;

	private List<BoosterpackData> activeBoosterpackDatas = new List<BoosterpackData>();

	private void Awake()
	{
		instance = this;
		AllQuests = GetAllQuests();
		foreach (Quest allQuest in AllQuests)
		{
			idToQuest[allQuest.Id] = allQuest;
		}
		sortedQuests = AllQuests.OrderBy((Quest x) => x.QuestGroup).ToList();
		Debug.Log($"{AllQuests.Count} Quests Found!");
	}

	private void Start()
	{
		UpdateCurrentQuests();
	}

	public void CheckSteamAchievements()
	{
		if (Application.isEditor)
		{
			return;
		}
		foreach (Quest allQuest in AllQuests)
		{
			if (allQuest.IsSteamAchievement && QuestIsComplete(allQuest))
			{
				AchievementHelper.UnlockAchievement(allQuest.Id);
			}
		}
	}

	private Quest GetQuestWithId(string id)
	{
		idToQuest.TryGetValue(id, out var value);
		return value;
	}

	public void UpdateCurrentQuests()
	{
		CurrentQuests.Clear();
		List<Quest> list = AllQuests.OrderBy((Quest x) => x.QuestGroup).ToList();
		for (int i = 0; i < list.Count; i++)
		{
			Quest quest = list[i];
			if (!QuestIsComplete(quest))
			{
				CurrentQuests.Add(quest);
			}
		}
		if (GameScreen.instance != null)
		{
			GameScreen.instance.UpdateQuestLog();
		}
	}

	public void CheckPacksUnlocked()
	{
		if (BoosterIsUnlocked(WorldManager.instance.GetBoosterData("structures"), allowDebug: false))
		{
			SpecialActionComplete("unlocked_all_packs");
		}
		if (BoosterIsUnlocked(WorldManager.instance.GetBoosterData("island_locations"), allowDebug: false))
		{
			SpecialActionComplete("unlocked_all_island_packs");
		}
		BoosterpackData boosterData = WorldManager.instance.GetBoosterData("cities_ideas_2");
		if (boosterData != null && BoosterIsUnlocked(boosterData, allowDebug: false))
		{
			SpecialActionComplete("unlocked_all_cities_packs");
		}
		foreach (BuyBoosterBox allBoosterBox in WorldManager.instance.AllBoosterBoxes)
		{
			if (allBoosterBox.Booster.IsUnlocked)
			{
				instance.SpecialActionComplete("unlocked_" + allBoosterBox.Booster.BoosterId + "_pack");
			}
		}
	}

	public bool QuestIsVisible(Quest quest)
	{
		int num = sortedQuests.IndexOf(quest);
		if (num == 0 || sortedQuests[num - 1].QuestGroup != quest.QuestGroup || quest.DefaultVisible)
		{
			return true;
		}
		if (quest.QuestGroup == QuestGroup.Island_Misc)
		{
			return true;
		}
		if (quest.QuestGroup == QuestGroup.Death_Misc)
		{
			return true;
		}
		if (quest.QuestGroup == QuestGroup.Equipment)
		{
			return true;
		}
		if (quest.QuestGroup == QuestGroup.Discover_Spirits && WorldManager.instance.CurrentRunVariables.FinishedDemon && QuestIsComplete(sortedQuests.Find((Quest x) => x.QuestGroup == quest.QuestGroup)))
		{
			return true;
		}
		if (QuestIsComplete(sortedQuests[num - 1]) || QuestIsComplete(sortedQuests[num]))
		{
			return true;
		}
		return false;
	}

	public bool BoosterIsUnlocked(BoosterpackData p, bool allowDebug = true)
	{
		if (allowDebug && Application.isEditor && DebugOptions.Default.DebugUnlockBoosters)
		{
			return true;
		}
		return RemainingQuestCountToComplete(p) <= 0;
	}

	public int RemainingQuestCountToComplete(BoosterpackData p)
	{
		return Mathf.Min(AllQuests.Count, p.MinAchievementCount) - GetCompletedQuestCount(p.BoosterLocation);
	}

	private List<BoosterpackData> CurrentlyActiveBoosterpacks()
	{
		List<BoosterpackData> boosterPackDatas = WorldManager.instance.BoosterPackDatas;
		activeBoosterpackDatas.Clear();
		foreach (BoosterpackData item in boosterPackDatas)
		{
			if (item.BoosterLocation == WorldManager.instance.CurrentBoard.Location && item.MinAchievementCount > 0)
			{
				activeBoosterpackDatas.Add(item);
			}
		}
		return activeBoosterpackDatas;
	}

	public BoosterpackData NextPackUnlock()
	{
		List<BoosterpackData> list = CurrentlyActiveBoosterpacks();
		int completedQuestCount = GetCompletedQuestCount(WorldManager.instance.CurrentBoard.Location);
		for (int i = 0; i < list.Count; i++)
		{
			if (completedQuestCount < list[i].MinAchievementCount)
			{
				return list[i];
			}
		}
		return null;
	}

	public BoosterpackData JustUnlockedPack()
	{
		List<BoosterpackData> list = CurrentlyActiveBoosterpacks();
		int completedQuestCount = GetCompletedQuestCount(WorldManager.instance.CurrentBoard.Location);
		for (int i = 0; i < list.Count; i++)
		{
			if (completedQuestCount == list[i].MinAchievementCount)
			{
				return list[i];
			}
		}
		return null;
	}

	public int GetCompletedQuestCount(Location loc)
	{
		return GetCompletedQuestCountOnLocation(loc);
	}

	private int GetCompletedQuestCountOnLocation(Location loc)
	{
		List<string> completedAchievementIds = WorldManager.instance.CurrentSave.CompletedAchievementIds;
		int num = 0;
		foreach (string item in completedAchievementIds)
		{
			Quest questWithId = GetQuestWithId(item);
			if (questWithId != null && questWithId.QuestLocation == loc)
			{
				num++;
			}
		}
		return num;
	}

	public bool QuestIsComplete(Quest quest)
	{
		return QuestIsComplete(quest.Id);
	}

	public bool QuestIsComplete(string id)
	{
		return WorldManager.instance.CurrentSave.CompletedAchievementIds.Contains(id);
	}

	public bool IsInactiveSpiritQuest(Quest quest, Location currentLocation)
	{
		Location questLocation = quest.QuestLocation;
		if (questLocation == Location.Death || questLocation == Location.Greed || questLocation == Location.Happiness)
		{
			if (WorldManager.instance.CurrentBoard == null)
			{
				return false;
			}
			return quest.QuestLocation != currentLocation;
		}
		return false;
	}

	public void ActionComplete(CardData card, string action, CardData focusCard = null)
	{
		bool flag = false;
		Location location = WorldManager.instance.CurrentBoard.Location;
		foreach (Quest currentQuest in CurrentQuests)
		{
			if (!IsInactiveSpiritQuest(currentQuest, location) && currentQuest.OnActionComplete != null && currentQuest.OnActionComplete(card, action))
			{
				if (focusCard == null)
				{
					MarkQuestComplete(currentQuest, card);
				}
				else
				{
					MarkQuestComplete(currentQuest, focusCard);
				}
				flag = true;
			}
		}
		if (flag)
		{
			UpdateCurrentQuests();
		}
	}

	public void SpecialActionComplete(string action, CardData card = null)
	{
		bool flag = false;
		Location location = WorldManager.instance.CurrentBoard.Location;
		foreach (Quest currentQuest in CurrentQuests)
		{
			if (!IsInactiveSpiritQuest(currentQuest, location) && currentQuest.OnSpecialAction != null && currentQuest.OnSpecialAction(action))
			{
				MarkQuestComplete(currentQuest, card);
				flag = true;
			}
		}
		if (flag)
		{
			UpdateCurrentQuests();
		}
	}

	public void CardCreated(CardData card)
	{
		bool flag = false;
		Location location = WorldManager.instance.CurrentBoard.Location;
		foreach (Quest currentQuest in CurrentQuests)
		{
			if (!IsInactiveSpiritQuest(currentQuest, location) && currentQuest.OnCardCreate != null && currentQuest.OnCardCreate(card))
			{
				MarkQuestComplete(currentQuest, card);
				flag = true;
			}
		}
		if (flag)
		{
			UpdateCurrentQuests();
		}
	}

	public void DebugUnlockAllQuests()
	{
		foreach (Quest allQuest in GetAllQuests())
		{
			MarkQuestComplete(allQuest);
		}
	}

	public bool AnyIslandQuestComplete()
	{
		return AllQuests.Any((Quest x) => x.QuestLocation == Location.Island && QuestIsComplete(x));
	}

	private void MarkQuestComplete(Quest quest, CardData card = null)
	{
		SaveGame currentSave = WorldManager.instance.CurrentSave;
		if (!currentSave.CompletedAchievementIds.Contains(quest.Id))
		{
			currentSave.CompletedAchievementIds.Add(quest.Id);
			WorldManager.instance.QuestsCompleted++;
			WorldManager.instance.QuestCompleted(quest);
			if (quest.IsSteamAchievement)
			{
				AchievementHelper.UnlockAchievement(quest.Id);
			}
		}
	}

	public static List<Quest> GetAllQuests()
	{
		List<Quest> list = new List<Quest>();
		FieldInfo[] fields = typeof(AllQuests).GetFields(BindingFlags.Static | BindingFlags.Public);
		foreach (FieldInfo fieldInfo in fields)
		{
			if (fieldInfo.FieldType == typeof(Quest))
			{
				Quest item = (Quest)fieldInfo.GetValue(null);
				list.Add(item);
			}
		}
		return list;
	}
}
