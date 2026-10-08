using System;
using System.Collections.Generic;

[Serializable]
public class SaveRound
{
	public List<SavedCard> SavedCards;

	public List<SavedBooster> SavedBoosters;

	public List<SavedBoosterBox> SavedBoosterBoxes;

	public List<SavedBoard> SavedBoards;

	public List<string> BoughtBoosterIds;

	public List<string> GivenCards;

	public int CurrentMonth;

	public int OldCurrentMonth;

	public float MonthTimer;

	public SavedMonth BoardMonths;

	public int QuestsCompleted;

	public int NewCardsFound;

	public int CitiesWellbeing = 15;

	public int CitiesConflictMonth;

	public CardEventType? CitiesDisaster;

	public string CurrentBoardId = "main";

	public RunOptions RunOptions;

	public RunVariables RunVariables;

	public int SaveVersion;

	public List<SerializedKeyValuePair> ExtraKeyValues = new List<SerializedKeyValuePair>();

	public List<SavedConflict> SavedConflicts;

	[NonSerialized]
	public string DebugId = "";
}
