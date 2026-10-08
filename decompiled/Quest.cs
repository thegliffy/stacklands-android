using System;

public class Quest
{
	private Location? _questLocation;

	public string DescriptionTermOverride;

	public string Id;

	public int RequiredCount = -1;

	public bool DefaultVisible;

	public bool ShowCompleteAnimation = true;

	public Func<CardData, string, bool> OnActionComplete;

	public Func<CardData, bool> OnCardCreate;

	public Func<string, bool> OnSpecialAction;

	public bool IsSteamAchievement;

	public bool PossibleInPeacefulMode = true;

	public QuestGroup QuestGroup = QuestGroup.Other;

	public string DescriptionTerm => "quest_" + Id + "_text";

	public string Description
	{
		get
		{
			if (DescriptionTermOverride != null)
			{
				if (RequiredCount != -1)
				{
					return SokLoc.Translate(DescriptionTermOverride, LocParam.Create("count", RequiredCount.ToString()));
				}
				return SokLoc.Translate(DescriptionTermOverride);
			}
			return SokLoc.Translate(DescriptionTerm);
		}
	}

	public Location QuestLocation
	{
		get
		{
			if (!_questLocation.HasValue)
			{
				if (QuestGroup.ToString().StartsWith("Island"))
				{
					_questLocation = Location.Island;
				}
				else if (QuestGroup.ToString().StartsWith("Forest"))
				{
					_questLocation = Location.Forest;
				}
				else if (QuestGroup.ToString().StartsWith("Death"))
				{
					_questLocation = Location.Death;
				}
				else if (QuestGroup.ToString().StartsWith("Greed"))
				{
					_questLocation = Location.Greed;
				}
				else if (QuestGroup.ToString().StartsWith("Happiness"))
				{
					_questLocation = Location.Happiness;
				}
				else if (QuestGroup.ToString().StartsWith("Cities"))
				{
					_questLocation = Location.Cities;
				}
				else
				{
					_questLocation = Location.Mainland;
				}
			}
			return _questLocation.Value;
		}
	}

	public bool IsMainQuest
	{
		get
		{
			if (QuestGroup != 0 && QuestGroup != QuestGroup.MainQuest && QuestGroup != QuestGroup.Island_Beginnings && QuestGroup != QuestGroup.Island_MainQuest && QuestGroup != QuestGroup.Forest_MainQuest && QuestGroup != QuestGroup.Death_MainQuest && QuestGroup != QuestGroup.Happiness_MainQuest && QuestGroup != QuestGroup.Greed_MainQuest && QuestGroup != QuestGroup.Happiness_Starter && QuestGroup != QuestGroup.Death_Starter && QuestGroup != QuestGroup.Death_MainQuest && QuestGroup != QuestGroup.Discover_Spirits && QuestGroup != QuestGroup.Greed_Starter && QuestGroup != QuestGroup.Cities_MainQuest)
			{
				return QuestGroup == QuestGroup.Cities_Starter;
			}
			return true;
		}
	}

	public Quest(string id)
	{
		Id = id;
	}
}
