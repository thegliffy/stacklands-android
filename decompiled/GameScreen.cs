using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameScreen : SokScreen
{
	public TextMeshProUGUI MoneyText;

	public TextMeshProUGUI FoodText;

	public TextMeshProUGUI CardText;

	public TextMeshProUGUI TimeText;

	public TextMeshProUGUI HappinessText;

	public TextMeshProUGUI EnergyText;

	public TextMeshProUGUI WellbeingText;

	public TextMeshProUGUI DollarText;

	public TextMeshProUGUI WorkerText;

	public TextMeshProUGUI InfoTitle;

	public TextMeshProUGUI InfoText;

	public GameObject InfoDividerPrefab;

	private List<GameObject> dividerList = new List<GameObject>();

	public RectTransform InfoLayoutGroup;

	public Image TimeFill;

	public GameObject InfoBox;

	public RectTransform ResourceRect;

	public RectTransform TimeRect;

	public RectTransform ViewRect;

	public TextMeshProUGUI Valuetext;

	public GameObject ValueParent;

	public GameObject FoodCardBox;

	public ShowInfoBox ShowInfoBoxMoney;

	public ShowInfoBox ShowInfoBoxFood;

	public ShowInfoBox ShowInfoBoxTime;

	public ShowInfoBox ShowInfoBoxCard;

	public ShowInfoBox ShowInfoBoxHappiness;

	public ShowInfoBox ShowInfoBoxEnergy;

	public ShowInfoBox ShowInfoBoxWellbeing;

	public ShowInfoBox ShowInfoBoxDollar;

	public ShowInfoBox ShowInfoBoxWorker;

	public ShowInfoBox ShowInfoBoxEnergyButton;

	public Image Crosshair;

	public static string InfoBoxTitle;

	public static string InfoBoxText;

	public Image DebugScreen;

	public CustomButton GameSpeedButton;

	public CustomButton ViewButton;

	public TextMeshProUGUI NextPackToUnlockText;

	public static GameScreen instance;

	public TextMeshProUGUI NoIdeasYetText;

	public RectTransform QuestsParent;

	public RectTransform QuestsTab;

	public RectTransform IdeasTab;

	public TextMeshProUGUI PausedText;

	public CustomButton QuestsButton;

	public CustomButton IdeasButton;

	public TMP_InputField IdeaSearchField;

	public RectTransform NotificationsParent;

	public CustomButton MinimizeButton;

	public GameObject IdeasTabNew;

	public GameObject QuestsTabNew;

	public Image FoldedNewIcon;

	public ScrollRect QuestsScrollRect;

	public ScrollRect IdeasScrollRect;

	private bool isMinimized;

	public bool ControllerIsInUI;

	private List<CustomButton> questButtons;

	private List<CustomButton> ideaButtons;

	private string previousInfoText = "";

	public string HappinessSummaryText;

	public string EnergySummaryText;

	public string WellbeingSummaryText;

	public RectTransform ViewDropdown;

	public CustomButton DefaultViewButton;

	public CustomButton EnergyViewButton;

	public CustomButton TransportViewButton;

	public CustomButton SewageViewButton;

	public CustomButton CalamityViewButton;

	private float pauseBlinkTimer;

	private bool gameSpeedButtonClicked;

	private bool questTabOpen = true;

	public List<AchievementElement> questElements;

	private List<QuestGroup> questGroupOrder = new List<QuestGroup>
	{
		QuestGroup.Starter,
		QuestGroup.MainQuest,
		QuestGroup.Island_Beginnings,
		QuestGroup.Island_Combat,
		QuestGroup.Island_Cooking,
		QuestGroup.Island_Misc,
		QuestGroup.Island_MainQuest,
		QuestGroup.Forest_MainQuest,
		QuestGroup.Fighting,
		QuestGroup.Equipment,
		QuestGroup.Cooking,
		QuestGroup.Exploration,
		QuestGroup.Resources,
		QuestGroup.Building,
		QuestGroup.Survival,
		QuestGroup.Discover_Spirits,
		QuestGroup.Other
	};

	public RectTransform IdeaElementsParent;

	private List<BlueprintGroup> groups = new List<BlueprintGroup>
	{
		BlueprintGroup.Basic,
		BlueprintGroup.Important,
		BlueprintGroup.Building,
		BlueprintGroup.Cooking,
		BlueprintGroup.Military,
		BlueprintGroup.Resources,
		BlueprintGroup.Island,
		BlueprintGroup.Sailing,
		BlueprintGroup.Fishing,
		BlueprintGroup.Happiness,
		BlueprintGroup.Greed,
		BlueprintGroup.Death,
		BlueprintGroup.Power,
		BlueprintGroup.Automation,
		BlueprintGroup.Landmark
	};

	private List<IdeaElement> ideaElements;

	private List<ExpandableLabel> ideaLabels;

	private int foundCount;

	public float prePauseSpeed = 1f;

	public RectTransform SideTransform;

	public ShowInfoBox MinimizeButtonInfoBox;

	private Dictionary<string, CardData> stackRequirements = new Dictionary<string, CardData>();

	private Dictionary<string, int> stackRequirementAmount = new Dictionary<string, int>();

	private float redTextBlinkTimer;

	private bool redBlink;

	public Image GameSpeedIcon;

	public override bool IsFrameRateUncapped => true;

	private void Awake()
	{
		instance = this;
		DebugScreen.gameObject.SetActive(value: false);
		GameSpeedButton.Clicked += delegate
		{
			gameSpeedButtonClicked = true;
		};
		ViewDropdown.gameObject.SetActive(value: false);
		ViewButton.Clicked += delegate
		{
			ViewDropdown.gameObject.SetActive(value: true);
		};
		DefaultViewButton.Clicked += delegate
		{
			SetView(ViewType.Default);
		};
		EnergyViewButton.Clicked += delegate
		{
			SetView(ViewType.Energy);
		};
		TransportViewButton.Clicked += delegate
		{
			SetView(ViewType.Transport);
		};
		SewageViewButton.Clicked += delegate
		{
			SetView(ViewType.Sewer);
		};
		CalamityViewButton.Clicked += delegate
		{
			SetView(ViewType.Calamity);
		};
		MinimizeButton.Clicked += delegate
		{
			ToggleMinimize();
		};
		IdeaSearchField.onValueChanged.AddListener(delegate
		{
			UpdateIdeasLog();
		});
		QuestsButton.Clicked += delegate
		{
			questTabOpen = true;
			QuestsTab.gameObject.SetActive(value: true);
			IdeasTab.gameObject.SetActive(value: false);
		};
		QuestsButton.ExplicitNavigationChanged += delegate(CustomButton cb, Navigation nav)
		{
			List<CustomButton> list2 = (questTabOpen ? questButtons : ideaButtons);
			nav.selectOnDown = ((list2 != null && list2.Count > 0) ? list2[0] : null);
			nav.selectOnRight = IdeasButton;
			return nav;
		};
		IdeasButton.ExplicitNavigationChanged += delegate(CustomButton cb, Navigation nav)
		{
			List<CustomButton> list = (questTabOpen ? questButtons : ideaButtons);
			nav.selectOnDown = ((list != null && list.Count > 0) ? list[0] : null);
			nav.selectOnLeft = QuestsButton;
			return nav;
		};
		IdeasButton.Clicked += delegate
		{
			questTabOpen = false;
			QuestsTab.gameObject.SetActive(value: false);
			IdeasTab.gameObject.SetActive(value: true);
		};
		GameSpeedButton.IsSelectableAction = () => false;
		MinimizeButton.IsSelectableAction = () => false;
		QuestsTab.gameObject.SetActive(value: true);
		IdeasTab.gameObject.SetActive(value: false);
		PausedText.gameObject.SetActive(value: false);
		NotificationsParent.gameObject.SetActive(value: true);
		SetViewdropdownTexts();
		InitIdeaElements();
		SokLoc.instance.LanguageChanged += Instance_LanguageChanged;
	}

	private void SetView(ViewType viewType)
	{
		WorldManager.instance.SetViewType(viewType);
		ViewDropdown.gameObject.SetActive(value: false);
	}

	public void CloseViewDropdown()
	{
		ViewDropdown.gameObject.SetActive(value: false);
	}

	private string GetIconForView(ViewType viewType)
	{
		return viewType switch
		{
			ViewType.Default => Icons.Card, 
			ViewType.Energy => Icons.Energy, 
			ViewType.Sewer => Icons.Sewer, 
			ViewType.Transport => Icons.Transport, 
			ViewType.Calamity => Icons.Calamity, 
			_ => throw new ArgumentException(), 
		};
	}

	private string GetLabelForViewType(ViewType viewType)
	{
		switch (viewType)
		{
		case ViewType.Default:
			return SokLoc.Translate("label_view_default");
		case ViewType.Energy:
			return SokLoc.Translate("label_view_energy");
		case ViewType.Sewer:
			return SokLoc.Translate("label_view_sewage");
		case ViewType.Calamity:
			return SokLoc.Translate("label_view_calamity");
		case ViewType.Transport:
			if (!(WorldManager.instance.GetCurrentBoardSafe().Id == "cities"))
			{
				return SokLoc.Translate("label_view_transport_default");
			}
			return SokLoc.Translate("label_view_transport");
		default:
			throw new ArgumentException();
		}
	}

	private void SetViewdropdownTexts()
	{
		DefaultViewButton.TextMeshPro.text = GetLabelForViewType(ViewType.Default) + GetIconForView(ViewType.Default);
		EnergyViewButton.TextMeshPro.text = GetLabelForViewType(ViewType.Energy) + GetIconForView(ViewType.Energy);
		TransportViewButton.TextMeshPro.text = GetLabelForViewType(ViewType.Transport) + GetIconForView(ViewType.Transport);
		SewageViewButton.TextMeshPro.text = GetLabelForViewType(ViewType.Sewer) + GetIconForView(ViewType.Sewer);
		CalamityViewButton.TextMeshPro.text = GetLabelForViewType(ViewType.Calamity) + GetIconForView(ViewType.Calamity);
	}

	public void OnBoardChange()
	{
		SetViewdropdownTexts();
	}

	private void Instance_LanguageChanged()
	{
		UpdateIdeasLog();
		UpdateQuestLog();
		SetViewdropdownTexts();
	}

	private void OnDestroy()
	{
		if (SokLoc.instance != null)
		{
			SokLoc.instance.LanguageChanged -= Instance_LanguageChanged;
		}
	}

	public DebugScreen GetDebugComponent()
	{
		return DebugScreen?.GetComponent<DebugScreen>();
	}

	public void SetQuestTab()
	{
		questTabOpen = true;
		QuestsTab.gameObject.SetActive(value: true);
		IdeasTab.gameObject.SetActive(value: false);
	}

	public void ScrollToQuest(Quest quest)
	{
		StartCoroutine(ScrollToQuestCoroutine(quest));
	}

	private IEnumerator ScrollToQuestCoroutine(Quest quest)
	{
		UpdateQuestLog();
		SetQuestTab();
		yield return null;
		ExpandableLabel expandableLabel = QuestsParent.GetComponentsInChildren<ExpandableLabel>().FirstOrDefault((ExpandableLabel x) => (QuestGroup)x.Tag == quest.QuestGroup);
		if (expandableLabel != null)
		{
			expandableLabel.SetExpanded(expanded: true);
		}
		AchievementElement achievementElement = questElements.FirstOrDefault((AchievementElement x) => x.MyQuest == quest);
		if (achievementElement != null)
		{
			GameCanvas.SetScrollRectPosition(QuestsScrollRect, achievementElement.transform as RectTransform);
		}
	}

	public void SetMinimize(bool minimized)
	{
		isMinimized = minimized;
		if (isMinimized)
		{
			QuestsTab.gameObject.SetActive(value: false);
			IdeasTab.gameObject.SetActive(value: false);
		}
		else
		{
			QuestsTab.gameObject.SetActive(questTabOpen);
			IdeasTab.gameObject.SetActive(!questTabOpen);
		}
	}

	public void ToggleMinimize()
	{
		SetMinimize(!isMinimized);
	}

	private void OnEnable()
	{
		if (!(QuestManager.instance == null))
		{
			UpdateQuestLog();
			UpdateIdeasLog();
		}
	}

	public void UpdateQuestLog()
	{
		if (QuestManager.instance == null)
		{
			return;
		}
		Dictionary<object, bool> dictionary = wasExpandedDict(QuestsParent.GetComponentsInChildren<ExpandableLabel>());
		IEnumerable<Quest> source = ((WorldManager.instance.CurrentBoard == null || WorldManager.instance.CurrentBoard.Id == "main") ? ((!QuestManager.instance.AllQuests.Any((Quest x) => x.QuestGroup == QuestGroup.Starter && !QuestManager.instance.QuestIsComplete(x))) ? QuestManager.instance.AllQuests.Where((Quest x) => x.QuestLocation != Location.Death && x.QuestLocation != Location.Greed && x.QuestLocation != Location.Happiness && x.QuestLocation != Location.Cities) : QuestManager.instance.AllQuests.Where((Quest x) => x.QuestGroup == QuestGroup.Starter)) : ((WorldManager.instance.CurrentBoard.Id == "island") ? ((!QuestManager.instance.AllQuests.Any((Quest x) => x.QuestGroup == QuestGroup.Island_Beginnings && !QuestManager.instance.QuestIsComplete(x))) ? QuestManager.instance.AllQuests.Where((Quest x) => x.QuestLocation != Location.Death && x.QuestLocation != Location.Greed && x.QuestLocation != Location.Happiness && x.QuestLocation != Location.Cities) : QuestManager.instance.AllQuests.Where((Quest x) => x.QuestGroup == QuestGroup.Island_Beginnings)) : ((!(WorldManager.instance.CurrentBoard.Id == "forest")) ? QuestManager.instance.AllQuests : QuestManager.instance.AllQuests.Where((Quest x) => x.QuestLocation == Location.Forest))));
		if (WorldManager.instance.CurrentRunVariables != null && !WorldManager.instance.CurrentRunVariables.VisitedIsland)
		{
			source = source.Where((Quest x) => x.QuestLocation != Location.Island);
		}
		if (WorldManager.instance.CurrentBoard?.Id == "happiness")
		{
			source = ((!QuestManager.instance.AllQuests.Any((Quest x) => x.QuestGroup == QuestGroup.Happiness_Starter && !QuestManager.instance.QuestIsComplete(x))) ? QuestManager.instance.AllQuests.Where((Quest x) => x.QuestLocation == Location.Happiness) : QuestManager.instance.AllQuests.Where((Quest x) => x.QuestGroup == QuestGroup.Happiness_Starter));
		}
		else if (WorldManager.instance.CurrentBoard?.Id == "greed")
		{
			source = ((!QuestManager.instance.AllQuests.Any((Quest x) => x.QuestGroup == QuestGroup.Greed_Starter && !QuestManager.instance.QuestIsComplete(x))) ? QuestManager.instance.AllQuests.Where((Quest x) => x.QuestLocation == Location.Greed) : QuestManager.instance.AllQuests.Where((Quest x) => x.QuestGroup == QuestGroup.Greed_Starter));
		}
		else if (WorldManager.instance.CurrentBoard?.Id == "death")
		{
			source = ((!QuestManager.instance.AllQuests.Any((Quest x) => x.QuestGroup == QuestGroup.Death_Starter && !QuestManager.instance.QuestIsComplete(x))) ? QuestManager.instance.AllQuests.Where((Quest x) => x.QuestLocation == Location.Death) : QuestManager.instance.AllQuests.Where((Quest x) => x.QuestGroup == QuestGroup.Death_Starter));
		}
		if (WorldManager.instance.CurrentBoard?.Id == "cities")
		{
			source = ((!QuestManager.instance.AllQuests.Any((Quest x) => x.QuestGroup == QuestGroup.Cities_Starter && !QuestManager.instance.QuestIsComplete(x))) ? QuestManager.instance.AllQuests.Where((Quest x) => x.QuestLocation == Location.Cities) : QuestManager.instance.AllQuests.Where((Quest x) => x.QuestGroup == QuestGroup.Cities_Starter));
			if (!WorldManager.instance.HasFoundCard("blueprint_barrack"))
			{
				source = source.Where((Quest x) => x.QuestGroup != QuestGroup.Cities_Freedom);
			}
		}
		bool flag = WorldManager.instance.CurrentRunVariables.FinishedDemon || QuestManager.instance.QuestIsComplete("kill_demon");
		if (!WorldManager.instance.IsSpiritDlcActive() || !flag)
		{
			source = source.Where((Quest x) => x.QuestGroup != QuestGroup.Discover_Spirits);
		}
		questElements = CreateQuestElements(QuestsParent, source.ToList());
		questButtons = (from x in QuestsParent.GetComponentsInChildren<CustomButton>()
			where x.enabled
			select x).ToList();
		for (int i = 0; i < questButtons.Count - 1; i++)
		{
			questButtons[i].ExplicitNavigationChanged += delegate(CustomButton cb, Navigation nav)
			{
				int num = questButtons.IndexOf(cb);
				nav.selectOnUp = ((num == 0) ? QuestsButton : questButtons[num - 1]);
				nav.selectOnDown = questButtons[num + 1];
				return nav;
			};
		}
		ExpandableLabel[] componentsInChildren = QuestsParent.GetComponentsInChildren<ExpandableLabel>();
		foreach (AchievementElement questElement in questElements)
		{
			if (questElement.IsNew)
			{
				dictionary[questElement.MyQuest.QuestGroup] = true;
			}
		}
		SetFromWasExpandedDict(componentsInChildren, dictionary);
	}

	private string GetAchievementGroupName(QuestGroup group)
	{
		string text = "questgroup_";
		return SokLoc.Translate(group switch
		{
			QuestGroup.Starter => text + "starter", 
			QuestGroup.MainQuest => text + "mainquest", 
			QuestGroup.Fighting => text + "fighting", 
			QuestGroup.Cooking => text + "cooking", 
			QuestGroup.Exploration => text + "exploration", 
			QuestGroup.Resources => text + "resources", 
			QuestGroup.Building => text + "building", 
			QuestGroup.Survival => text + "survival", 
			QuestGroup.Other => text + "other", 
			QuestGroup.Island_Misc => text + "island", 
			_ => text + group.ToString().ToLower(), 
		});
	}

	private List<AchievementElement> CreateQuestElements(RectTransform parent, List<Quest> quests, bool addLabels = true)
	{
		List<AchievementElement> list = new List<AchievementElement>();
		foreach (Transform item in parent)
		{
			if (!item.name.StartsWith("DontDestroy"))
			{
				UnityEngine.Object.Destroy(item.gameObject);
			}
		}
		List<Quest> source = new List<Quest>(quests);
		quests = (from x in quests
			orderby !x.IsMainQuest, questGroupOrder.IndexOf(x.QuestGroup)
			select x).ToList();
		quests.RemoveAll((Quest x) => !QuestManager.instance.QuestIsVisible(x));
		Quest quest = null;
		ExpandableLabel expandableLabel = null;
		bool flag = (from x in quests
			group x by x.QuestGroup).Count() > 1;
		for (int i = 0; i < quests.Count; i++)
		{
			Quest cur = quests[i];
			Quest quest2 = ((i == quests.Count - 1) ? null : quests[i + 1]);
			if (addLabels)
			{
				if (flag && (quest == null || quest.IsMainQuest != cur.IsMainQuest))
				{
					RectTransform rectTransform = UnityEngine.Object.Instantiate(PrefabManager.instance.NormalLabelPrefab);
					rectTransform.transform.SetParentClean(parent);
					rectTransform.GetComponent<CustomButton>().enabled = false;
					rectTransform.GetComponent<Image>().enabled = false;
					TextMeshProUGUI componentInChildren = rectTransform.GetComponentInChildren<TextMeshProUGUI>();
					componentInChildren.fontStyle = FontStyles.Bold;
					componentInChildren.text = (cur.IsMainQuest ? SokLoc.Translate("label_main_quests") : SokLoc.Translate("label_side_quests"));
				}
				if (quest == null || quest.QuestGroup != cur.QuestGroup)
				{
					expandableLabel = UnityEngine.Object.Instantiate(PrefabManager.instance.AchievementElementLabelPrefab).GetComponent<ExpandableLabel>();
					expandableLabel.transform.SetParentClean(parent);
					expandableLabel.Tag = cur.QuestGroup;
					int num = source.Count((Quest x) => x.QuestGroup == cur.QuestGroup);
					int num2 = source.Count((Quest x) => x.QuestGroup == cur.QuestGroup && QuestManager.instance.QuestIsComplete(x));
					string achievementGroupName = GetAchievementGroupName(cur.QuestGroup);
					achievementGroupName = ((num != num2) ? (achievementGroupName + $" ({num2}/{num})") : (achievementGroupName + " " + Icons.Checkmark));
					expandableLabel.SetText(achievementGroupName);
					if (flag)
					{
						expandableLabel.SetExpanded(expanded: false);
					}
				}
			}
			AchievementElement achievementElement = UnityEngine.Object.Instantiate(PrefabManager.instance.AchievementElementPrefab);
			achievementElement.SetQuest(cur);
			expandableLabel.Children.Add(achievementElement.gameObject);
			if (flag)
			{
				achievementElement.gameObject.SetActive(value: false);
			}
			achievementElement.transform.SetParentClean(parent);
			list.Add(achievementElement);
			if ((quest2 == null || cur.QuestGroup != quest2.QuestGroup) && source.Count((Quest x) => x.QuestGroup == cur.QuestGroup && !QuestManager.instance.QuestIsVisible(x)) > 0)
			{
				AchievementElement achievementElement2 = UnityEngine.Object.Instantiate(PrefabManager.instance.EmptyAchievementElementPrefab);
				achievementElement2.transform.SetParentClean(parent);
				expandableLabel.Children.Add(achievementElement2.gameObject);
				if (flag)
				{
					achievementElement2.gameObject.SetActive(value: false);
				}
			}
			quest = cur;
		}
		return list;
	}

	private Dictionary<object, bool> wasExpandedDict(ExpandableLabel[] labels)
	{
		Dictionary<object, bool> dictionary = new Dictionary<object, bool>();
		foreach (ExpandableLabel expandableLabel in labels)
		{
			dictionary[expandableLabel.Tag] = expandableLabel.IsExpanded;
		}
		return dictionary;
	}

	private void SetFromWasExpandedDict(ExpandableLabel[] labels, Dictionary<object, bool> wasExpanded)
	{
		foreach (ExpandableLabel expandableLabel in labels)
		{
			if (wasExpanded.ContainsKey(expandableLabel.Tag))
			{
				expandableLabel.SetExpanded(wasExpanded[expandableLabel.Tag]);
			}
		}
	}

	public void UpdateIdeaElements()
	{
		UpdateIdeasLog();
	}

	public void InitIdeaElements()
	{
		List<IKnowledge> list = new List<IKnowledge>();
		IEnumerable<Rumor> collection = WorldManager.instance.CardDataPrefabs.OfType<Rumor>();
		list.AddRange(collection);
		List<Blueprint> list2 = new List<Blueprint>(WorldManager.instance.BlueprintPrefabs);
		list2.RemoveAll((Blueprint x) => x.HideFromIdeasTab);
		list.AddRange(list2.Cast<IKnowledge>());
		List<IKnowledge> source = new List<IKnowledge>(list);
		list = (from k in list
			orderby groups.IndexOf(k.Group), k.KnowledgeName
			select k).ToList();
		_ = list.Count;
		IKnowledge knowledge = null;
		ExpandableLabel expandableLabel = null;
		ideaElements = new List<IdeaElement>();
		ideaLabels = new List<ExpandableLabel>();
		for (int i = 0; i < list.Count; i++)
		{
			IKnowledge cur = list[i];
			if (knowledge == null || knowledge.Group != cur.Group)
			{
				expandableLabel = UnityEngine.Object.Instantiate(PrefabManager.instance.AchievementElementLabelPrefab).GetComponent<ExpandableLabel>();
				expandableLabel.transform.SetParentClean(IdeaElementsParent);
				expandableLabel.Tag = cur.Group;
				source.Count((IKnowledge k) => k.Group == cur.Group);
				source.Count((IKnowledge k) => k.Group == cur.Group && KnowledgeWasFound(k));
				expandableLabel.SetText(GetBlueprintGroupText(cur.Group));
				expandableLabel.SetCallback(UpdateIdeaElements);
				ideaLabels.Add(expandableLabel);
			}
			IdeaElement ideaElement = UnityEngine.Object.Instantiate(PrefabManager.instance.IdeaElementPrefab);
			ideaElement.transform.SetParentClean(IdeaElementsParent);
			expandableLabel.Children.Add(ideaElement.gameObject);
			ideaElement.SetKnowledge(cur);
			ideaElements.Add(ideaElement);
			knowledge = cur;
		}
	}

	public void UpdateIdeasLog()
	{
		string searchTerm = "";
		if (!string.IsNullOrEmpty(IdeaSearchField.text))
		{
			searchTerm = IdeaSearchField.text;
		}
		List<IKnowledge> currentKnowledges = new List<IKnowledge>();
		IEnumerable<Rumor> collection = WorldManager.instance.CardDataPrefabs.OfType<Rumor>();
		currentKnowledges.AddRange(collection);
		List<Blueprint> list = new List<Blueprint>(WorldManager.instance.BlueprintPrefabs);
		list.RemoveAll((Blueprint x) => x.HideFromIdeasTab);
		currentKnowledges.AddRange(list.Cast<IKnowledge>());
		new List<IKnowledge>(currentKnowledges);
		currentKnowledges = currentKnowledges.OrderBy((IKnowledge k) => groups.IndexOf(k.Group)).ThenBy((IKnowledge x) => x.KnowledgeName).ToList();
		if (WorldManager.instance.CurrentBoard?.Id == "cities")
		{
			currentKnowledges = currentKnowledges.Where((IKnowledge x) => WorldManager.instance.GameDataLoader.GetCardFromId(x.CardId).CardUpdateType == CardUpdateType.Cities).ToList();
		}
		else
		{
			currentKnowledges = currentKnowledges.Where((IKnowledge x) => WorldManager.instance.GameDataLoader.GetCardFromId(x.CardId).CardUpdateType != CardUpdateType.Cities).ToList();
		}
		_ = currentKnowledges.Count;
		Dictionary<object, bool> dictionary = wasExpandedDict(IdeaElementsParent.GetComponentsInChildren<ExpandableLabel>());
		foreach (IdeaElement element in ideaElements)
		{
			IKnowledge knowledge = currentKnowledges.Find((IKnowledge x) => x.CardId == element.MyKnowledge.CardId);
			if (knowledge == null)
			{
				element.gameObject.SetActive(value: false);
				continue;
			}
			element.SetKnowledge(knowledge);
			if (KnowledgeWasFound(element.MyKnowledge))
			{
				if (element.IsNew)
				{
					dictionary[element.MyKnowledge.Group] = true;
				}
				if (!string.IsNullOrEmpty(searchTerm))
				{
					if (KnowledgeMatchesSearch(element.MyKnowledge, searchTerm))
					{
						element.gameObject.SetActive(value: true);
						continue;
					}
				}
				else if (dictionary.ContainsKey(element.MyKnowledge.Group) && dictionary[element.MyKnowledge.Group])
				{
					element.gameObject.SetActive(value: true);
					continue;
				}
			}
			element.gameObject.SetActive(value: false);
		}
		foreach (ExpandableLabel ideaLabel in ideaLabels)
		{
			ideaLabel.SetText(GetBlueprintGroupText((BlueprintGroup)ideaLabel.Tag));
			if (ideaLabel.Children.Count((GameObject x) => HasFoundKnowledge(x, out var knowledge3) && currentKnowledges.Contains(knowledge3)) > 0)
			{
				if (string.IsNullOrEmpty(searchTerm))
				{
					ideaLabel.gameObject.SetActive(value: true);
					ideaLabel.IsExpanded = dictionary.ContainsKey(ideaLabel.Tag) && dictionary[ideaLabel.Tag];
					continue;
				}
				if (ideaLabel.Children.Count((GameObject x) => HasFoundKnowledge(x, out var _) && KnowledgeMatchesSearch(x.GetComponent<IdeaElement>().MyKnowledge, searchTerm) && currentKnowledges.Contains(x.GetComponent<IdeaElement>().MyKnowledge)) > 0)
				{
					ideaLabel.gameObject.SetActive(value: true);
					ideaLabel.IsExpanded = true;
					continue;
				}
			}
			ideaLabel.gameObject.SetActive(value: false);
		}
		foundCount = ideaElements.Where((IdeaElement x) => KnowledgeWasFound(x.MyKnowledge)).Count();
		ideaButtons = IdeaElementsParent.GetComponentsInChildren<CustomButton>().ToList();
		for (int i = 0; i < ideaButtons.Count - 1; i++)
		{
			ideaButtons[i].ExplicitNavigationChanged += delegate(CustomButton cb, Navigation nav)
			{
				int num = ideaButtons.IndexOf(cb);
				if (num == 0)
				{
					nav.selectOnUp = IdeasButton;
				}
				else if (ideaButtons[num - 1].gameObject.activeInHierarchy)
				{
					nav.selectOnUp = ideaButtons[num - 1];
				}
				else
				{
					nav.selectOnUp = getFirstActiveFromIndexUp(num - 1, ideaButtons);
				}
				if (ideaButtons[num + 1].gameObject.activeInHierarchy)
				{
					nav.selectOnDown = ideaButtons[num + 1];
				}
				else
				{
					nav.selectOnDown = getFirstActiveFromIndexDown(num + 1, ideaButtons);
				}
				return nav;
			};
		}
		NoIdeasYetText.gameObject.SetActive(foundCount == 0);
	}

	private bool HasFoundKnowledge(GameObject obj, out IKnowledge knowledge)
	{
		knowledge = null;
		IdeaElement component = obj.GetComponent<IdeaElement>();
		if (component == null)
		{
			return false;
		}
		knowledge = component.MyKnowledge;
		return KnowledgeWasFound(component.MyKnowledge);
	}

	private bool KnowledgeMatchesSearch(IKnowledge knowledge, string searchTerm)
	{
		string text;
		string value;
		if (ShouldKeepAccents())
		{
			text = knowledge.KnowledgeName.ToLower().Replace(" ", "");
			value = searchTerm.ToLower().Replace(" ", "");
		}
		else
		{
			text = RemoveAccents(knowledge.KnowledgeName.ToLower().Replace(" ", ""));
			value = RemoveAccents(searchTerm.ToLower().Replace(" ", ""));
		}
		if (text.Contains(value))
		{
			return true;
		}
		return false;
	}

	private bool ShouldKeepAccents()
	{
		if (!(SokLoc.instance.CurrentLanguage == "Chinese (Traditional)") && !(SokLoc.instance.CurrentLanguage == "Chinese (Simplified)") && !(SokLoc.instance.CurrentLanguage == "Japanese"))
		{
			return SokLoc.instance.CurrentLanguage == "Korean";
		}
		return true;
	}

	private static string RemoveAccents(string input)
	{
		string s = input.Normalize(NormalizationForm.FormKD);
		byte[] bytes = Encoding.GetEncoding(Encoding.ASCII.CodePage, new EncoderReplacementFallback(""), new DecoderReplacementFallback("")).GetBytes(s);
		return Encoding.ASCII.GetString(bytes);
	}

	private CustomButton getFirstActiveFromIndexDown(int index, List<CustomButton> buttonList)
	{
		for (int i = index; i < buttonList.Count - 1; i++)
		{
			if (buttonList[i].gameObject.activeInHierarchy)
			{
				return buttonList[i];
			}
		}
		return null;
	}

	private CustomButton getFirstActiveFromIndexUp(int index, List<CustomButton> buttonList)
	{
		for (int num = index; num >= 0; num--)
		{
			if (buttonList[num].gameObject.activeInHierarchy)
			{
				return buttonList[num];
			}
		}
		return null;
	}

	private bool KnowledgeWasFound(IKnowledge knowledge)
	{
		return WorldManager.instance.CurrentSave.FoundCardIds.Contains(knowledge.CardId);
	}

	private string GetBlueprintGroupText(BlueprintGroup group)
	{
		return SokLoc.Translate("ideagroup_" + group.ToString().ToLower());
	}

	private bool CompletedFirstAchievement()
	{
		return QuestManager.instance.QuestIsComplete(QuestManager.instance.AllQuests[0]);
	}

	public void SetControllerInUI(bool inUI)
	{
		if (ControllerIsInUI != inUI)
		{
			ControllerIsInUI = inUI;
			if (!ControllerIsInUI)
			{
				EventSystem.current.SetSelectedGameObject(null);
				WorldManager.instance.SpeedUp = prePauseSpeed;
			}
			else
			{
				prePauseSpeed = WorldManager.instance.SpeedUp;
				WorldManager.instance.SpeedUp = 0f;
			}
		}
	}

	private void Update()
	{
		if (WorldManager.instance.CurseIsActive(CurseType.Happiness))
		{
			ShowInfoBoxHappiness.gameObject.SetActiveFast(active: true);
		}
		else
		{
			ShowInfoBoxHappiness.gameObject.SetActiveFast(active: false);
		}
		ShowInfoBoxEnergy.gameObject.SetActiveFast(active: false);
		if (WorldManager.instance.CurrentBoard.Id == "cities")
		{
			ViewRect.gameObject.SetActiveFast(active: true);
			SewageViewButton.gameObject.SetActiveFast(active: true);
			CalamityViewButton.gameObject.SetActiveFast(active: true);
			EnergyViewButton.gameObject.SetActiveFast(active: true);
			ShowInfoBoxMoney.gameObject.SetActiveFast(active: false);
			ShowInfoBoxDollar.gameObject.SetActiveFast(active: true);
			ShowInfoBoxWorker.gameObject.SetActiveFast(active: true);
			ShowInfoBoxWellbeing.gameObject.SetActiveFast(active: true);
		}
		else
		{
			if (WorldManager.instance.GetCardCount<RoadBuilder>() > 0)
			{
				ViewRect.gameObject.SetActiveFast(active: true);
				SewageViewButton.gameObject.SetActiveFast(active: false);
				CalamityViewButton.gameObject.SetActiveFast(active: false);
				EnergyViewButton.gameObject.SetActiveFast(active: false);
			}
			else
			{
				ViewRect.gameObject.SetActiveFast(active: false);
			}
			ShowInfoBoxMoney.gameObject.SetActiveFast(active: true);
			ShowInfoBoxDollar.gameObject.SetActiveFast(active: false);
			ShowInfoBoxWorker.gameObject.SetActiveFast(active: false);
			ShowInfoBoxWellbeing.gameObject.SetActiveFast(active: false);
		}
		questButtons.RemoveAll((CustomButton x) => x == null);
		ideaButtons.RemoveAll((CustomButton x) => x == null);
		foreach (GameObject divider in dividerList)
		{
			divider.SetActive(value: false);
		}
		if (InputController.instance.PanelCollapse_Triggered())
		{
			ToggleMinimize();
		}
		if (InputController.instance.ActivateUI_Triggered())
		{
			SetControllerInUI(!ControllerIsInUI);
		}
		if (!InputController.instance.CurrentSchemeIsController)
		{
			SetControllerInUI(inUI: false);
		}
		QuestsTabNew.gameObject.SetActiveFast(questElements.Any((AchievementElement x) => x.IsNew));
		IdeasTabNew.gameObject.SetActiveFast(ideaElements.Any((IdeaElement x) => x.IsNew && x.gameObject.activeSelf));
		FoldedNewIcon.gameObject.SetActiveFast(isMinimized && (QuestsTabNew.gameObject.activeInHierarchy || IdeasTabNew.gameObject.activeInHierarchy));
		MinimizeButtonInfoBox.InfoBoxTitle = SokLoc.Translate("label_toggle_panel_title");
		MinimizeButtonInfoBox.InfoBoxText = SokLoc.Translate("label_toggle_panel_text", Extensions.LocParam_Action("panel_collapse"));
		UpdateSidePanelPosition();
		MinimizeButton.transform.localScale = (isMinimized ? Vector3.one : new Vector3(-1f, 1f, 1f));
		QuestsButton.Image.color = (QuestsTab.gameObject.activeInHierarchy ? ColorManager.instance.BackgroundColor : ColorManager.instance.InactiveBackgroundColor);
		IdeasButton.Image.color = (IdeasTab.gameObject.activeInHierarchy ? ColorManager.instance.BackgroundColor : ColorManager.instance.InactiveBackgroundColor);
		MoneyText.text = $"{WorldManager.instance.GetGoldCount(includeInChest: true)} {Icons.Gold}";
		DollarText.text = $"{WorldManager.instance.GetDollarCount(includeInChest: true)}{Icons.Dollar}";
		int foodCount = WorldManager.instance.GetFoodCount();
		int requiredFoodCount = WorldManager.instance.GetRequiredFoodCount();
		int cardCount = WorldManager.instance.GetCardCount();
		int maxCardCount = WorldManager.instance.GetMaxCardCount();
		int happinessCount = WorldManager.instance.GetHappinessCount();
		int requiredHappinessCount = WorldManager.instance.GetRequiredHappinessCount();
		int wellbeing = CitiesManager.instance.Wellbeing;
		CityState cityState = CitiesManager.instance.CityState;
		string text = GameCanvas.FormatTime(WorldManager.instance.MonthTime - WorldManager.instance.MonthTimer);
		ShowInfoBoxTime.InfoBoxTitle = SokLoc.Translate("label_time");
		ShowInfoBoxTime.InfoBoxText = SokLoc.Translate("label_time_infobox", LocParam.Create("time_left", text.ToString()), Extensions.LocParam_Action("time_pause"), Extensions.LocParam_Action("time_toggle"));
		ShowInfoBoxEnergyButton.InfoBoxTitle = SokLoc.Translate("label_energy_view");
		ShowInfoBoxEnergyButton.InfoBoxText = SokLoc.Translate("label_energy_view_infobox", Extensions.LocParam_Action("toggle_view"));
		FoodText.text = $"{foodCount}/{requiredFoodCount} {Icons.Food}";
		ShowInfoBoxMoney.InfoBoxTitle = SokLoc.Translate("label_coin_infobox_title");
		ShowInfoBoxMoney.InfoBoxText = SokLoc.Translate("label_coin_infobox_text");
		ShowInfoBoxFood.InfoBoxTitle = SokLoc.Translate("cardtype_food");
		ShowInfoBoxFood.InfoBoxText = SokLoc.Translate("label_food_infobox", LocParam.Create("foodicon", Icons.Food), LocParam.Create("required_food_count", requiredFoodCount.ToString()), LocParam.Create("food_count", foodCount.ToString()));
		CardText.text = $"{cardCount}/{maxCardCount} {Icons.Card}";
		ShowInfoBoxCard.InfoBoxTitle = SokLoc.Translate("label_card_cap");
		ShowInfoBoxCard.InfoBoxText = SokLoc.Translate("label_cards_infobox", LocParam.Create("cardicon", Icons.Card), LocParam.Create("card_count", cardCount.ToString()), LocParam.Create("max_card_count", maxCardCount.ToString()));
		if (WorldManager.instance.ForestMoonEnabled)
		{
			FoodCardBox.SetActiveFast(active: false);
		}
		else
		{
			FoodCardBox.SetActiveFast(active: true);
		}
		if (foodCount < requiredFoodCount || cardCount > maxCardCount || WorldManager.instance.DebugNoFoodEnabled || WorldManager.instance.DebugNoEnergyEnabled || wellbeing < 40 || (WorldManager.instance.CurseIsActive(CurseType.Happiness) && happinessCount < requiredHappinessCount))
		{
			redTextBlinkTimer += Time.deltaTime;
			if (redTextBlinkTimer >= 0.5f)
			{
				redTextBlinkTimer = 0f;
				redBlink = !redBlink;
			}
		}
		else
		{
			redTextBlinkTimer = 0f;
			redBlink = false;
		}
		if (foodCount < requiredFoodCount || WorldManager.instance.DebugNoFoodEnabled)
		{
			FoodText.color = (redBlink ? ColorManager.instance.RedTextColor : ColorManager.instance.TextColor);
			ShowInfoBox showInfoBoxFood = ShowInfoBoxFood;
			showInfoBoxFood.InfoBoxText = showInfoBoxFood.InfoBoxText + ". " + SokLoc.Translate("label_food_infobox_warning", LocParam.Create("foodicon", Icons.Food));
		}
		else
		{
			FoodText.color = ColorManager.instance.TextColor;
		}
		if (cardCount > maxCardCount)
		{
			CardText.color = (redBlink ? ColorManager.instance.RedTextColor : ColorManager.instance.TextColor);
			ShowInfoBox showInfoBoxCard = ShowInfoBoxCard;
			showInfoBoxCard.InfoBoxText = showInfoBoxCard.InfoBoxText + ". " + SokLoc.Translate("label_cards_infobox_warning", LocParam.Create("cardicon", Icons.Card));
		}
		else
		{
			CardText.color = ColorManager.instance.TextColor;
		}
		if (WorldManager.instance.CurrentRunOptions.IsPeacefulMode)
		{
			TimeText.text = SokLoc.Translate("label_timetext_peaceful", LocParam.Create("moon", WorldManager.instance.CurrentMonth.ToString()));
		}
		else if (WorldManager.instance.ForestMoonEnabled)
		{
			string text2 = SokLoc.Translate("label_timetext", LocParam.Create("moon", "??"));
			string text3 = SokLoc.Translate("label_wave", LocParam.Create("wave", WorldManager.instance.CurrentRunVariables.ForestWave.ToString()));
			TimeText.text = text2 + " - " + text3;
		}
		else
		{
			TimeText.text = SokLoc.Translate("label_timetext", LocParam.Create("moon", WorldManager.instance.CurrentMonth.ToString()));
		}
		if (HappinessText.isActiveAndEnabled)
		{
			HappinessText.text = $"{happinessCount}/{requiredHappinessCount} {Icons.Happiness}";
			ShowInfoBoxHappiness.InfoBoxTitle = SokLoc.Translate("cardtype_happiness");
			ShowInfoBoxHappiness.InfoBoxText = SokLoc.Translate("label_happiness_infobox", LocParam.Create("happinessicon", Icons.Happiness), LocParam.Create("required_happiness_count", requiredHappinessCount.ToString()), LocParam.Create("happiness_count", happinessCount.ToString()));
			if (happinessCount < requiredHappinessCount || WorldManager.instance.DebugNoFoodEnabled)
			{
				HappinessText.color = (redBlink ? ColorManager.instance.RedTextColor : ColorManager.instance.TextColor);
				ShowInfoBox showInfoBoxHappiness = ShowInfoBoxHappiness;
				showInfoBoxHappiness.InfoBoxText = showInfoBoxHappiness.InfoBoxText + ". " + SokLoc.Translate("label_happiness_infobox_warning", LocParam.Create("happinessicon", Icons.Happiness));
			}
			else
			{
				HappinessText.color = ColorManager.instance.TextColor;
			}
			HappinessSummaryText = ShowInfoBoxHappiness.InfoBoxText;
		}
		if (WorkerText.isActiveAndEnabled)
		{
			int num = CitiesManager.instance.HousingConsumers.Sum((HousingConsumer x) => x.GetHousingSpaceRequired());
			int num2 = WorldManager.instance.GetCards<Apartment>().Sum((Apartment x) => x.HousingSpace);
			WorkerText.text = $"{num}/{num2}{Icons.Housing}";
			ShowInfoBoxWorker.InfoBoxTitle = SokLoc.Translate("label_info_worker_space_title");
			ShowInfoBoxWorker.InfoBoxText = SokLoc.Translate("label_info_worker_space_text", LocParam.Create("workers", num.ToString()), LocParam.Create("space", num2.ToString()), LocParam.Create("icon", Icons.Housing));
			if (num > num2)
			{
				ShowInfoBox showInfoBoxWorker = ShowInfoBoxWorker;
				showInfoBoxWorker.InfoBoxText = showInfoBoxWorker.InfoBoxText + ". " + SokLoc.Translate("label_info_worker_space_text_1", LocParam.Create("icon", Icons.Housing));
				WorkerText.color = (redBlink ? ColorManager.instance.RedTextColor : ColorManager.instance.TextColor);
			}
			else
			{
				WorkerText.color = ColorManager.instance.TextColor;
			}
		}
		if (WellbeingText.isActiveAndEnabled)
		{
			ShowInfoBoxWellbeing.InfoBoxTitle = SokLoc.Translate("label_wellbeing");
			ShowInfoBoxWellbeing.InfoBoxText = SokLoc.Translate("label_wellbeing_infobox_" + cityState.ToString().ToLower());
			WellbeingText.text = $"{wellbeing} {Icons.Wellbeing}";
			if (CitiesManager.instance.Wellbeing < 10)
			{
				WellbeingText.color = (redBlink ? ColorManager.instance.RedTextColor : ColorManager.instance.TextColor);
			}
			else
			{
				WellbeingText.color = ColorManager.instance.TextColor;
			}
		}
		if (ShowInfoBoxDollar.gameObject.activeSelf)
		{
			ShowInfoBoxDollar.InfoBoxTitle = SokLoc.Translate("label_dollar");
			ShowInfoBoxDollar.InfoBoxText = SokLoc.Translate("label_dollar_infobox", LocParam.Create("amount", WorldManager.instance.GetDollarCount(includeInChest: true).ToString()), LocParam.Create("icon", Icons.Dollar));
		}
		TimeFill.fillAmount = (WorldManager.instance.ForestMoonEnabled ? 0f : (WorldManager.instance.MonthTimer / WorldManager.instance.MonthTime));
		BoosterpackData boosterpackData = QuestManager.instance.NextPackUnlock();
		NextPackToUnlockText.gameObject.SetActiveFast(boosterpackData != null && CompletedFirstAchievement());
		if (boosterpackData != null)
		{
			int pluralCount = QuestManager.instance.RemainingQuestCountToComplete(boosterpackData);
			NextPackToUnlockText.text = SokLoc.Translate("label_complete_more_quests", LocParam.Plural("remaining", pluralCount));
		}
		GameCard gameCard = null;
		if (WorldManager.instance.DraggingCard != null)
		{
			gameCard = WorldManager.instance.DraggingCard;
		}
		else if (WorldManager.instance.HoveredCard != null)
		{
			gameCard = WorldManager.instance.HoveredCard;
		}
		string text4 = "";
		if (gameCard == null)
		{
			Boosterpack boosterpack = null;
			if (WorldManager.instance.DraggingDraggable is Boosterpack)
			{
				boosterpack = WorldManager.instance.DraggingDraggable as Boosterpack;
			}
			else if (WorldManager.instance.HoveredDraggable is Boosterpack)
			{
				boosterpack = WorldManager.instance.HoveredDraggable as Boosterpack;
			}
			if (boosterpack != null)
			{
				InfoTitle.text = boosterpack.Name ?? "";
				InfoText.text = SokLoc.Translate("label_click_this_pack");
			}
			else
			{
				InfoTitle.text = "";
				InfoText.text = "";
			}
		}
		else if (WorldManager.instance.CurrentHoverable == null)
		{
			CardValue stackValue = WorldManager.instance.GetStackValue(gameCard);
			List<GameCard> allCardsInStack = gameCard.GetAllCardsInStack();
			if (!gameCard.IsPartOfStack())
			{
				InfoTitle.text = gameCard.CardData.FullName;
				string text5 = gameCard.CardData.Description;
				if (gameCard.CardData.RequirementHolders.Count > 0 && WorldManager.instance.GetCurrentBoardSafe().Id == "cities")
				{
					string requirementDescription = gameCard.CardData.GetRequirementDescription(gameCard, 1, onlyShowCurrentlySatisfied: false);
					if (!string.IsNullOrEmpty(requirementDescription))
					{
						text5 = text5 + "\\d<i>" + SokLoc.Translate("label_at_end_moon") + "</i>\n" + requirementDescription;
					}
				}
				InfoText.text = text5;
				GameCard cardWithStatusInStack = gameCard.GetCardWithStatusInStack();
				if (cardWithStatusInStack != null)
				{
					InfoTitle.text = cardWithStatusInStack.Status + "..";
					InfoText.text = GameCanvas.FormatTimeLeft(cardWithStatusInStack.TargetTimerTime - cardWithStatusInStack.CurrentTimerTime) + " \n\n" + gameCard.CardData.Description;
				}
				else if (gameCard.CardData.GetValue() > 0)
				{
					if (!(WorldManager.instance.NearbyCardTarget is SellBox))
					{
						if (WorldManager.instance.NearbyCardTarget is BuyBoosterBox)
						{
							_ = (BuyBoosterBox)WorldManager.instance.NearbyCardTarget;
						}
						else
						{
							text4 = stackValue.ToValueString(WorldManager.instance.CurrentBoard);
						}
					}
				}
				else if (gameCard.CardData.GetValue() == -1)
				{
					text4 = SokLoc.Translate("label_cant_be_sold");
				}
			}
			else
			{
				GameCard cardWithStatusInStack2 = gameCard.GetCardWithStatusInStack();
				GameCard cardInCombatInStack = gameCard.GetCardInCombatInStack();
				if (cardWithStatusInStack2 != null)
				{
					InfoTitle.text = cardWithStatusInStack2.Status + "..";
					InfoText.text = GameCanvas.FormatTimeLeft(cardWithStatusInStack2.TargetTimerTime - cardWithStatusInStack2.CurrentTimerTime);
				}
				else if ((bool)cardInCombatInStack)
				{
					InfoTitle.text = gameCard.CardData.FullName;
					InfoText.text = gameCard.CardData.Description;
				}
				else
				{
					InfoTitle.text = SokLoc.Translate("label_stack_of_cards");
					InfoText.text = gameCard.GetStackSummary();
					string text6 = "";
					if (allCardsInStack.Any((GameCard x) => x.CardData.RequirementHolders.Count > 0) && WorldManager.instance.GetCurrentBoardSafe().Id == "cities")
					{
						stackRequirements.Clear();
						stackRequirementAmount.Clear();
						foreach (GameCard item in allCardsInStack)
						{
							if (stackRequirements.ContainsKey(item.CardData.Id))
							{
								stackRequirementAmount[item.CardData.Id]++;
							}
							else if (item.CardData.RequirementHolders.Count > 0)
							{
								stackRequirements[item.CardData.Id] = item.CardData;
								stackRequirementAmount[item.CardData.Id] = 1;
							}
						}
						bool flag = true;
						bool flag2 = stackRequirements.Count > 1;
						foreach (CardData value in stackRequirements.Values)
						{
							int num3 = stackRequirementAmount[value.Id];
							string requirementDescription2 = value.GetRequirementDescription(value.MyGameCard, num3, onlyShowCurrentlySatisfied: false);
							if (!string.IsNullOrEmpty(requirementDescription2))
							{
								if (flag)
								{
									flag = false;
									text6 = text6 + "\\d<i>" + SokLoc.Translate("label_at_end_moon") + "</i>";
								}
								else
								{
									text6 += "\n";
								}
								if (flag2)
								{
									string text7 = ((num3 != 1) ? $"{num3}x {SokLoc.Translate(value.NameTerm)}" : (value.Name ?? ""));
									text6 = text6 + "\n<i>(" + text7 + ")</i>\n" + requirementDescription2;
								}
								else
								{
									text6 = text6 + "\n" + requirementDescription2;
								}
							}
						}
					}
					InfoText.text += text6;
					CardData cardData = null;
					foreach (GameCard item2 in allCardsInStack)
					{
						if (item2.CardData.GetValue() == -1)
						{
							cardData = item2.CardData;
						}
					}
					if (cardData != null)
					{
						text4 = SokLoc.Translate("label_cant_be_sold");
						if (WorldManager.instance.NearbyCardTarget is BuyBoosterBox)
						{
							_ = (BuyBoosterBox)WorldManager.instance.NearbyCardTarget;
						}
					}
					else if (WorldManager.instance.NearbyCardTarget is SellBox)
					{
						TextMeshProUGUI infoText = InfoText;
						infoText.text = infoText.text + "\n" + SokLoc.Translate("label_drop_to_sell", LocParam.Create("value", stackValue.ToValueString(WorldManager.instance.CurrentBoard)));
					}
					else
					{
						text4 = stackValue.ToValueString(WorldManager.instance.CurrentBoard);
					}
				}
			}
			if (InfoText.text.Contains("\\d"))
			{
				string[] array = InfoText.text.Split(new string[1] { "\\d" }, StringSplitOptions.None);
				for (int i = 0; i < array.Length; i++)
				{
					string text8 = array[i];
					if (i == 0)
					{
						InfoText.text = text8;
						continue;
					}
					GameObject obj = FindOrInstantiateDivider();
					obj.SetActiveFast(active: true);
					TextMeshProUGUI componentInChildren = obj.GetComponentInChildren<TextMeshProUGUI>();
					if (componentInChildren != null)
					{
						componentInChildren.text = text8;
					}
				}
			}
		}
		if (ControllerIsInUI)
		{
			InfoText.text = InfoBoxText;
			InfoTitle.text = InfoBoxTitle;
		}
		if (WorldManager.instance.CurrentHoverable != null)
		{
			InfoTitle.text = (InfoBoxTitle = WorldManager.instance.CurrentHoverable.GetTitle());
			InfoText.text = (InfoBoxText = WorldManager.instance.CurrentHoverable.GetDescription());
			text4 = null;
		}
		if (InputController.instance.CurrentSchemeIsMouseKeyboard && InputController.instance.InputCount > 0 && (!InputController.instance.GetInput(0) || !GameCanvas.instance.PositionIsOverUI(InputController.instance.GetInputPosition(0))))
		{
			CloseViewDropdown();
		}
		ViewButton.TextMeshPro.text = GetLabelForViewType(WorldManager.instance.CurrentView) + GetIconForView(WorldManager.instance.CurrentView);
		Crosshair.gameObject.SetActiveFast(InputController.instance.CurrentSchemeIsController);
		IdeaSearchField.gameObject.SetActiveFast(foundCount > 0 && !InputController.instance.CurrentSchemeIsController);
		ValueParent.gameObject.SetActiveFast(!string.IsNullOrEmpty(text4));
		Valuetext.text = text4;
		bool flag3 = true;
		if (WorldManager.instance.InAnimation || GameCanvas.instance.ModalIsOpen)
		{
			flag3 = false;
		}
		if (flag3)
		{
			if (InputController.instance.TimeToggleTriggered())
			{
				if (WorldManager.instance.SpeedUp == 0f)
				{
					WorldManager.instance.SpeedUp = 1f;
				}
				else if (WorldManager.instance.SpeedUp == 1f)
				{
					WorldManager.instance.SpeedUp = 5f;
				}
				else if (WorldManager.instance.SpeedUp == 5f)
				{
					WorldManager.instance.SpeedUp = 1f;
				}
			}
			if (gameSpeedButtonClicked)
			{
				gameSpeedButtonClicked = false;
				if (WorldManager.instance.SpeedUp == 0f)
				{
					WorldManager.instance.SpeedUp = 1f;
				}
				else if (WorldManager.instance.SpeedUp == 1f)
				{
					WorldManager.instance.SpeedUp = 5f;
				}
				else if (WorldManager.instance.SpeedUp == 5f)
				{
					WorldManager.instance.SpeedUp = 0f;
				}
			}
			if (InputController.instance.Time1_Triggered())
			{
				WorldManager.instance.SpeedUp = 1f;
			}
			if (InputController.instance.Time2_Triggered())
			{
				WorldManager.instance.SpeedUp = 5f;
			}
			if (InputController.instance.Time3_Triggered())
			{
				prePauseSpeed = WorldManager.instance.SpeedUp;
				WorldManager.instance.SpeedUp = 0f;
			}
			if (InputController.instance.TimePauseTriggered())
			{
				TimePause();
			}
		}
		if (WorldManager.instance.SpeedUp == 5f)
		{
			GameSpeedIcon.sprite = SpriteManager.instance.Speed10;
		}
		else if (WorldManager.instance.SpeedUp == 1f)
		{
			GameSpeedIcon.sprite = SpriteManager.instance.Speed1;
		}
		else if (WorldManager.instance.SpeedUp == 0f)
		{
			GameSpeedIcon.sprite = SpriteManager.instance.Speed0;
		}
		bool flag4 = WorldManager.instance.SpeedUp == 0f;
		PausedText.gameObject.SetActive(flag4);
		if (flag4)
		{
			QuestManager.instance.SpecialActionComplete("pause_game");
			pauseBlinkTimer += Time.deltaTime;
			if (pauseBlinkTimer >= 0.5f)
			{
				PausedText.enabled = !PausedText.enabled || !AccessibilityScreen.FlashingPausedEnabled;
				pauseBlinkTimer = 0f;
			}
		}
		else
		{
			pauseBlinkTimer = 0f;
		}
		_ = previousInfoText != InfoText.text;
		previousInfoText = InfoText.text;
	}

	private GameObject FindOrInstantiateDivider()
	{
		foreach (GameObject divider in dividerList)
		{
			if (!divider.activeSelf)
			{
				return divider;
			}
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(InfoDividerPrefab, InfoLayoutGroup);
		dividerList.Add(gameObject);
		return gameObject;
	}

	public void UpdateSidePanelPosition()
	{
		Vector2 anchoredPosition = SideTransform.anchoredPosition;
		anchoredPosition.x = (isMinimized ? (-340f) : 5f);
		SideTransform.anchoredPosition = anchoredPosition;
	}

	public void TimePause()
	{
		if (WorldManager.instance.SpeedUp >= 1f)
		{
			prePauseSpeed = WorldManager.instance.SpeedUp;
			WorldManager.instance.SpeedUp = 0f;
		}
		else
		{
			WorldManager.instance.SpeedUp = (ControllerIsInUI ? prePauseSpeed : ((prePauseSpeed != 0f) ? prePauseSpeed : 1f));
		}
	}

	public void AddNotification(string title, string text, Action onClicked = null)
	{
		NotificationElement notificationElement = UnityEngine.Object.Instantiate(PrefabManager.instance.NotificationElementPrefab);
		notificationElement.transform.SetParent(NotificationsParent);
		notificationElement.transform.localScale = Vector3.one;
		notificationElement.transform.localPosition = Vector3.zero;
		notificationElement.transform.localRotation = Quaternion.identity;
		notificationElement.NotificationText.text = text;
		notificationElement.NotificationTitle.text = title;
		notificationElement.OnClicked = onClicked;
		if (NotificationsParent.childCount > 5)
		{
			UnityEngine.Object.Destroy(NotificationsParent.GetChild(0).gameObject);
		}
	}

	private void LateUpdate()
	{
		if (InfoTitle.text == "")
		{
			InfoTitle.text = InfoBoxTitle;
			InfoText.text = InfoBoxText;
		}
		InfoBoxText = "";
		InfoBoxTitle = "";
	}
}
