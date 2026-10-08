using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveGame
{
	[NonSerialized]
	public string SaveId = "";

	[NonSerialized]
	public string FullPath = "";

	public SaveRound LastPlayedRound;

	public List<string> CompletedAchievementIds = new List<string>();

	public List<string> FoundCardIds = new List<string>();

	public List<string> FoundBoosterIds = new List<string>();

	public List<string> NewCardopediaIds = new List<string>();

	public List<string> NewKnowledgeIds = new List<string>();

	public List<string> SeenQuestIds = new List<string>();

	public List<SerializedKeyValuePair> ExtraKeyValues = new List<SerializedKeyValuePair>();

	public List<string> DisabledMods = new List<string>();

	public bool GotIslandIntroPack;

	public long LastSavedUtcTicks;

	public bool FinishedGreed;

	public bool FinishedDeath;

	public bool FinishedHappiness;

	public DateTime LastSavedUtc
	{
		get
		{
			return new DateTime(LastSavedUtcTicks);
		}
		set
		{
			LastSavedUtcTicks = value.Ticks;
		}
	}

	public static SaveGame LoadFromString(string json, string saveId)
	{
		SaveGame saveGame;
		if (!string.IsNullOrEmpty(json))
		{
			try
			{
				saveGame = JsonUtility.FromJson<SaveGame>(json);
				if (saveGame.LastPlayedRound != null && saveGame.LastPlayedRound.SavedCards.Count == 0 && saveGame.LastPlayedRound.SavedBoosters.Count == 0)
				{
					saveGame.LastPlayedRound = null;
				}
			}
			catch
			{
				saveGame = new SaveGame();
			}
		}
		else
		{
			saveGame = new SaveGame();
		}
		saveGame.SaveId = saveId;
		return saveGame;
	}
}
