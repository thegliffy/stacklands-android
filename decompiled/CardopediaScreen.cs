using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardopediaScreen : SokScreen
{
	public RectTransform EntriesParent;

	public CustomButton BackButton;

	public ScrollRect ScrollRect;

	public TMP_InputField SearchField;

	public ExpandableLabelCardopedia LabelPrefab;

	public CardopediaEntryElement CardopediaEntryPrefab;

	public CardopediaEntryElement HoveredEntry;

	public TextMeshProUGUI CardFoundAmount;

	public TextMeshProUGUI CardDescription;

	public Transform TargetCardPos;

	public Transform CardopediaBackground;

	public CustomButton All;

	public CustomButton Main;

	public CustomButton Island;

	public CustomButton Forest;

	public CustomButton Order;

	public CustomButton Spirit;

	public CustomButton Cities;

	public CustomButton Modded;

	private List<CustomButton> tabButtons;

	private CustomButton activeTab;

	private CardUpdateType? activeCardUpdateType;

	private List<CardopediaEntryElement> entries = new List<CardopediaEntryElement>();

	private List<ExpandableLabelCardopedia> labels = new List<ExpandableLabelCardopedia>();

	private CardopediaEntryElement lastHoveredEntry;

	private GameCard demoCard;

	private List<object> listChildren = new List<object>();

	public static CardopediaScreen instance;

	private bool SearchDisabled;

	private int totalFoundCount;

	private int currentTotalCardCount;

	public bool IsSearching => !string.IsNullOrEmpty(SearchField.text);

	private void Awake()
	{
		instance = this;
		tabButtons = new List<CustomButton> { All, Main, Island, Forest, Order, Spirit, Cities, Modded };
		BackButton.Clicked += delegate
		{
			CardopediaBackground.gameObject.SetActive(value: false);
			ClearScreen();
			if (WorldManager.instance.CurrentGameState == WorldManager.GameState.InMenu)
			{
				GameCanvas.instance.SetScreen<MainMenu>();
			}
			else
			{
				GameCanvas.instance.SetScreen<PauseScreen>();
			}
		};
		SearchField.onValueChanged.AddListener(delegate(string value)
		{
			FilterEntries();
			foreach (ExpandableLabelCardopedia label in labels)
			{
				if (GetActiveLabelChildrenCount(label) > 0 && !string.IsNullOrEmpty(value))
				{
					label.SetExpanded(expanded: true);
					label.ShowChildrenCardopedia();
				}
			}
		});
		SokLoc.instance.LanguageChanged += Instance_LanguageChanged;
		AddTabListeners();
		CardopediaBackground = GameCamera.instance.transform.Find("CardopediaBackground");
		TargetCardPos = GameCamera.instance.transform.Find("TargetCardPos");
		CardopediaBackground.gameObject.SetActive(value: false);
		CreateEntries();
		if (!PlatformHelper.HasModdingSupport)
		{
			Modded.gameObject.SetActive(value: false);
		}
	}

	private void OnDestroy()
	{
		if (SokLoc.instance != null)
		{
			SokLoc.instance.LanguageChanged -= Instance_LanguageChanged;
		}
	}

	private void Instance_LanguageChanged()
	{
		foreach (CardopediaEntryElement entry in entries)
		{
			entry.UpdateText();
		}
		if (demoCard != null)
		{
			demoCard.CardData.OnLanguageChange();
		}
		UpdateLabels();
	}

	public void RefreshCardopedia()
	{
		foreach (CardopediaEntryElement entry in entries)
		{
			entry.SetCardData(entry.MyCardData);
			entry.UpdateText();
		}
		UpdateLabels();
	}

	private void OnEnable()
	{
		RefreshCardopedia();
		CardDescription.transform.parent.gameObject.SetActive(value: false);
		CardopediaBackground.gameObject.SetActive(value: true);
		totalFoundCount = DetermineFoundCount();
		SwitchActiveTab(All);
		ScrollRect.verticalNormalizedPosition = 1f;
	}

	private int DetermineFoundCount(CardUpdateType? updateType = null)
	{
		List<string> foundCardIds = WorldManager.instance.CurrentSave.FoundCardIds;
		HashSet<string> hashSet = new HashSet<string>();
		foreach (string item in foundCardIds)
		{
			if (!hashSet.Contains(item))
			{
				hashSet.Add(item);
			}
		}
		int num = 0;
		List<CardData> list = WorldManager.instance.CardDataPrefabs;
		if (updateType.HasValue)
		{
			list = list.Where((CardData x) => x.CardUpdateType == updateType).ToList();
		}
		foreach (CardData item2 in list)
		{
			if (!item2.HideFromCardopedia && hashSet.Contains(item2.Id))
			{
				num++;
			}
		}
		return num;
	}

	private void AddTabListeners()
	{
		if (activeTab == null)
		{
			SwitchActiveTab(All);
		}
		foreach (CustomButton tab in tabButtons)
		{
			tab.Clicked += delegate
			{
				SwitchActiveTab(tab);
			};
			tab.ExplicitNavigationChanged += delegate(CustomButton but, Navigation nav)
			{
				nav.selectOnUp = null;
				nav.selectOnDown = GetFirstSelectableInList();
				return nav;
			};
		}
	}

	private Selectable GetFirstSelectableInList()
	{
		return labels.FirstOrDefault((ExpandableLabelCardopedia x) => x.gameObject.activeInHierarchy).MyButton;
	}

	private void SwitchActiveTab(CustomButton tab)
	{
		activeTab = tab;
		ScrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
		SearchDisabled = false;
		CardFoundAmount.gameObject.SetActive(value: true);
		if (activeTab == All)
		{
			activeCardUpdateType = null;
			FilterEntriesCardUpdateType(null);
		}
		else if (activeTab == Main)
		{
			activeCardUpdateType = CardUpdateType.Main;
			FilterEntriesCardUpdateType(CardUpdateType.Main);
		}
		else if (activeTab == Island)
		{
			activeCardUpdateType = CardUpdateType.Island;
			FilterEntriesCardUpdateType(CardUpdateType.Island);
		}
		else if (activeTab == Forest)
		{
			activeCardUpdateType = CardUpdateType.Forest;
			FilterEntriesCardUpdateType(CardUpdateType.Forest);
		}
		else if (activeTab == Order)
		{
			activeCardUpdateType = CardUpdateType.Order;
			FilterEntriesCardUpdateType(CardUpdateType.Order);
		}
		else if (activeTab == Spirit)
		{
			activeCardUpdateType = CardUpdateType.Spirit;
			FilterEntriesCardUpdateType(CardUpdateType.Spirit);
			if (!WorldManager.instance.IsSpiritDlcActive())
			{
				SetTempDemoCard(WorldManager.instance.CardDataPrefabs.Find((CardData x) => x.Id == "card_display_spirit_dlc"));
				ScrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
				SearchDisabled = true;
				CardFoundAmount.gameObject.SetActive(value: false);
			}
		}
		else if (activeTab == Cities)
		{
			activeCardUpdateType = CardUpdateType.Cities;
			FilterEntriesCardUpdateType(CardUpdateType.Cities);
			if (!WorldManager.instance.IsCitiesDlcActive())
			{
				SetTempDemoCard(WorldManager.instance.CardDataPrefabs.Find((CardData x) => x.Id == "display_2000_dlc"));
				ScrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
				SearchDisabled = true;
				CardFoundAmount.gameObject.SetActive(value: false);
			}
		}
		else if (activeTab == Modded)
		{
			activeCardUpdateType = CardUpdateType.Mod;
			FilterEntriesCardUpdateType(CardUpdateType.Mod);
		}
		currentTotalCardCount = WorldManager.instance.CardDataPrefabs.Count((CardData x) => !x.HideFromCardopedia);
		if (activeTab != All)
		{
			currentTotalCardCount = WorldManager.instance.CardDataPrefabs.Count((CardData x) => !x.HideFromCardopedia && x.CardUpdateType == activeCardUpdateType);
			SearchField.text = "";
		}
	}

	private void FilterEntriesCardUpdateType(CardUpdateType? cardUpdateType)
	{
		foreach (CardopediaEntryElement entry in entries)
		{
			if (!cardUpdateType.HasValue || entry.MyCardData.CardUpdateType == cardUpdateType)
			{
				entry.IsFilteredUpdate = true;
			}
			else
			{
				entry.IsFilteredUpdate = false;
			}
		}
		UpdateLabels();
		UpdateEntries();
	}

	private void FilterEntries()
	{
		string text = SearchField.text;
		if (!string.IsNullOrEmpty(text))
		{
			if (activeTab != All)
			{
				SwitchActiveTab(All);
			}
			text = text.ToLower().Replace(" ", "");
			foreach (CardopediaEntryElement entry in entries)
			{
				if (entry.MyCardData.Name.ToLower().Replace(" ", "").Contains(text))
				{
					entry.IsFiltered = true;
				}
				else
				{
					entry.IsFiltered = false;
				}
			}
		}
		else
		{
			foreach (CardopediaEntryElement entry2 in entries)
			{
				entry2.IsFiltered = true;
			}
		}
		UpdateLabels();
	}

	public void UpdateLabels()
	{
		foreach (ExpandableLabelCardopedia label in labels)
		{
			label.ShowChildrenCardopedia();
			if (IsSearching)
			{
				if (GetActiveLabelChildrenCountSearch(label) > 0)
				{
					CardType type2 = label.Children[0].MyCardData.MyCardType;
					int num = WorldManager.instance.CardDataPrefabs.Count((CardData x) => x.MyCardType == type2 && !x.HideFromCardopedia);
					label.SetText(CardTypeToText(type2) + $" ({GetActiveLabelChildrenCountSearch(label)}/{num})");
					label.gameObject.SetActive(value: true);
					continue;
				}
			}
			else if (label.Children.Count((CardopediaEntryElement x) => x.IsFilteredUpdate) > 0)
			{
				CardType type = label.Children[0].MyCardData.MyCardType;
				int num2 = (activeCardUpdateType.HasValue ? WorldManager.instance.CardDataPrefabs.Count((CardData x) => x.MyCardType == type && !x.HideFromCardopedia && x.CardUpdateType == activeCardUpdateType) : WorldManager.instance.CardDataPrefabs.Count((CardData x) => x.MyCardType == type && !x.HideFromCardopedia));
				label.SetText(CardTypeToText(type) + $" ({GetActiveLabelChildrenCount(label)}/{num2})");
				label.gameObject.SetActive(value: true);
				continue;
			}
			label.gameObject.SetActive(value: false);
		}
		totalFoundCount = DetermineFoundCount(activeCardUpdateType);
	}

	private int GetActiveLabelChildrenCountSearch(ExpandableLabelCardopedia label)
	{
		return label.Children.Count((CardopediaEntryElement x) => x.IsFiltered && x.wasFound);
	}

	private int GetActiveLabelChildrenCount(ExpandableLabelCardopedia label)
	{
		return label.Children.Count((CardopediaEntryElement x) => x.IsFilteredUpdate && x.wasFound);
	}

	public void UpdateEntries()
	{
		float verticalNormalizedPosition = ScrollRect.verticalNormalizedPosition;
		FilterEntries();
		UpdatePositions();
		ScrollRect.verticalNormalizedPosition = verticalNormalizedPosition;
		UpdatePositions();
	}

	private void CreateEntries()
	{
		List<CardData> cardDataPrefabs = WorldManager.instance.CardDataPrefabs;
		cardDataPrefabs = (from x in cardDataPrefabs
			orderby x.MyCardType, x.FullName
			select x).ToList();
		cardDataPrefabs.RemoveAll((CardData x) => x.HideFromCardopedia);
		new List<Transform>();
		foreach (Transform item in EntriesParent)
		{
			Object.Destroy(item.gameObject);
		}
		ExpandableLabelCardopedia expandableLabelCardopedia = null;
		labels = new List<ExpandableLabelCardopedia>();
		entries.Clear();
		listChildren.Clear();
		for (int j = 0; j < cardDataPrefabs.Count; j++)
		{
			CardData c = cardDataPrefabs[j];
			if (j == 0 || cardDataPrefabs[j - 1].MyCardType != cardDataPrefabs[j].MyCardType)
			{
				ExpandableLabelCardopedia label = Object.Instantiate(LabelPrefab);
				label.transform.SetParentClean(EntriesParent);
				int num = cardDataPrefabs.Count((CardData x) => x.MyCardType == c.MyCardType);
				int num2 = cardDataPrefabs.Count((CardData x) => x.MyCardType == c.MyCardType && WorldManager.instance.CurrentSave.FoundCardIds.Contains(x.Id));
				label.SetText(CardTypeToText(cardDataPrefabs[j].MyCardType) + $" ({num2}/{num})");
				label.Tag = cardDataPrefabs[j].MyCardType;
				label.SetCallback(delegate
				{
					float num3 = 0f - label.transform.localPosition.y - EntriesParent.localPosition.y;
					UpdateEntries();
					Vector3 localPosition = EntriesParent.transform.localPosition;
					localPosition.y = 0f - label.transform.localPosition.y - num3;
					EntriesParent.transform.localPosition = localPosition;
				});
				label.SetExpanded(expanded: false);
				label.MyButton.ExplicitNavigationChanged += delegate(CustomButton cb, Navigation nav)
				{
					if (cb == GetFirstSelectableInList())
					{
						nav.selectOnUp = activeTab;
					}
					Selectable selectOnLeft2 = (nav.selectOnRight = null);
					nav.selectOnLeft = selectOnLeft2;
					return nav;
				};
				listChildren.Add(label);
				labels.Add(label);
				expandableLabelCardopedia = label;
			}
			CardopediaEntryElement cardopediaEntryElement = Object.Instantiate(CardopediaEntryPrefab);
			cardopediaEntryElement.transform.SetParentClean(EntriesParent);
			cardopediaEntryElement.SetCardData(c);
			cardopediaEntryElement.IsEnabled = false;
			cardopediaEntryElement.IsFiltered = false;
			cardopediaEntryElement.IsFilteredUpdate = true;
			cardopediaEntryElement.Button.ExplicitNavigationChanged += delegate(CustomButton cb, Navigation nav)
			{
				Selectable selectOnLeft = (nav.selectOnRight = null);
				nav.selectOnLeft = selectOnLeft;
				return nav;
			};
			expandableLabelCardopedia.Children.Add(cardopediaEntryElement);
			entries.Add(cardopediaEntryElement);
			listChildren.Add(cardopediaEntryElement);
		}
		foreach (ExpandableLabelCardopedia i in labels)
		{
			if (entries.Any((CardopediaEntryElement e) => e.IsNew && e.MyCardData.MyCardType == (CardType)i.Tag))
			{
				i.SetExpanded(expanded: true);
			}
		}
	}

	private string CardTypeToText(CardType type)
	{
		return type.TranslateEnum();
	}

	private void OnDisable()
	{
		SearchField.text = string.Empty;
		ClearScreen();
	}

	private void ClearScreen()
	{
		if (demoCard != null)
		{
			Object.Destroy(demoCard.gameObject);
		}
		CardDescription.transform.parent.gameObject.SetActive(value: false);
		lastHoveredEntry = null;
		if (CardopediaBackground != null)
		{
			CardopediaBackground.gameObject.SetActive(value: false);
		}
	}

	private void Update()
	{
		HoveredEntry = null;
		if (GameCanvas.instance.ScreenIsInteractable<CardopediaScreen>())
		{
			foreach (CardopediaEntryElement entry in entries)
			{
				if (entry.Button.IsHovered || entry.Button.IsSelected)
				{
					HoveredEntry = entry;
				}
			}
		}
		if (lastHoveredEntry != null)
		{
			lastHoveredEntry.Button.Image.color = ColorManager.instance.ButtonColor;
		}
		if (HoveredEntry != null)
		{
			HoveredEntry.Button.Image.color = ColorManager.instance.HoverButtonColor;
		}
		UpdatePositions();
		if (lastHoveredEntry != HoveredEntry && HoveredEntry != null)
		{
			if (demoCard != null)
			{
				Object.Destroy(demoCard.gameObject);
			}
			demoCard = Object.Instantiate(PrefabManager.instance.GameCardPrefab);
			CardData cardData = Object.Instantiate(HoveredEntry.MyCardData);
			cardData.transform.SetParent(demoCard.transform);
			demoCard.CardData = cardData;
			cardData.MyGameCard = demoCard;
			demoCard.FaceUp = HoveredEntry.wasFound;
			demoCard.IsDemoCard = true;
			demoCard.SetDemoCardRotation();
			demoCard.CardData.UpdateCardText();
			demoCard.UpdateCardPalette();
			cardData.UpdateCard();
			demoCard.ForceUpdate();
		}
		if (demoCard != null)
		{
			Vector3 position = TargetCardPos.position;
			demoCard.transform.position = (demoCard.TargetPosition = position);
		}
		if (HoveredEntry != null)
		{
			CardDescription.transform.parent.gameObject.SetActive(value: true);
			if (HoveredEntry.wasFound)
			{
				demoCard.CardData.UpdateCardText();
				string dropSummaryFromCard = GetDropSummaryFromCard(HoveredEntry.MyCardData);
				string description = demoCard.CardData.Description;
				description = description.Replace("\\d", "\n\n");
				if (!string.IsNullOrEmpty(dropSummaryFromCard) && HoveredEntry.MyCardData.MyCardType != CardType.Locations)
				{
					description = description + "\n\n" + dropSummaryFromCard;
				}
				if (HoveredEntry.MyCardData is Blueprint blueprint)
				{
					description = blueprint.GetText();
				}
				CardDescription.text = description;
			}
			else
			{
				CardDescription.text = SokLoc.Translate("label_card_not_found");
			}
		}
		SearchField.gameObject.SetActive(!InputController.instance.CurrentSchemeIsController && !SearchDisabled);
		CardFoundAmount.text = SokLoc.Translate("label_cards_found", LocParam.Create("found", totalFoundCount.ToString()), LocParam.Create("total", currentTotalCardCount.ToString()));
		lastHoveredEntry = HoveredEntry;
		UpdateTabs();
	}

	private void UpdateTabs()
	{
		foreach (CustomButton tabButton in tabButtons)
		{
			if (tabButton.gameObject.activeInHierarchy)
			{
				bool flag = tabButton == activeTab;
				Color color = (tabButton.IsSelected ? ColorManager.instance.BackgroundColor2 : ((!flag) ? ColorManager.instance.InactiveBackgroundColor : ColorManager.instance.BackgroundColor));
				tabButton.Image.color = color;
			}
		}
	}

	public void UpdatePositions()
	{
		int num = 0;
		Vector2 sizeDelta = EntriesParent.sizeDelta;
		Vector2 vector = EntriesParent.localPosition;
		Rect rect = EntriesParent.rect;
		float height = ((RectTransform)EntriesParent.parent).rect.height;
		float num2 = 0f - vector.y - height * 0.5f;
		for (int i = 0; i < listChildren.Count; i++)
		{
			object obj = listChildren[i];
			bool flag = false;
			RectTransform rectTransform = null;
			if (obj is ExpandableLabelCardopedia expandableLabelCardopedia)
			{
				flag = expandableLabelCardopedia.gameObject.activeInHierarchy;
				rectTransform = (RectTransform)expandableLabelCardopedia.transform;
			}
			if (obj is CardopediaEntryElement cardopediaEntryElement)
			{
				flag = cardopediaEntryElement.IsEnabled;
				rectTransform = (RectTransform)cardopediaEntryElement.transform;
				cardopediaEntryElement.Button.Image.raycastTarget = cardopediaEntryElement.IsEnabled;
			}
			if (flag)
			{
				Vector3 localPosition = rectTransform.localPosition;
				localPosition.x = 0f;
				localPosition.y = (float)(-num) * 50f;
				rectTransform.localPosition = localPosition;
				Vector2 sizeDelta2 = rectTransform.sizeDelta;
				sizeDelta2.x = rect.width;
				rectTransform.sizeDelta = sizeDelta2;
				num++;
			}
			else
			{
				Vector3 position = new Vector3(1000f, 1000f);
				rectTransform.position = position;
			}
			if (obj is CardopediaEntryElement cardopediaEntryElement2)
			{
				bool flag2 = Mathf.Abs(rectTransform.localPosition.y - num2) < height * 0.75f;
				cardopediaEntryElement2.Cull(!cardopediaEntryElement2.IsEnabled || !flag2);
			}
		}
		sizeDelta.y = (float)num * 50f;
		EntriesParent.sizeDelta = sizeDelta;
	}

	private string GetDropSummaryFromCard(CardData cardData)
	{
		if (cardData is Harvestable)
		{
			return BoosterpackData.GetSummaryFromAllCards(cardData.GetPossibleDrops(), "label_can_drop");
		}
		if (cardData is Enemy)
		{
			return BoosterpackData.GetSummaryFromAllCards(cardData.GetPossibleDrops(), "label_can_drop");
		}
		return "";
	}

	private void SetTempDemoCard(CardData data)
	{
		if (demoCard != null)
		{
			Object.Destroy(demoCard.gameObject);
		}
		demoCard = Object.Instantiate(PrefabManager.instance.GameCardPrefab);
		CardData cardData = Object.Instantiate(data);
		cardData.transform.SetParent(demoCard.transform);
		demoCard.CardData = cardData;
		cardData.MyGameCard = demoCard;
		demoCard.FaceUp = true;
		demoCard.IsDemoCard = true;
		demoCard.SetDemoCardRotation();
		demoCard.UpdateCardPalette();
		cardData.UpdateCard();
		demoCard.ForceUpdate();
		CardDescription.transform.parent.gameObject.SetActive(value: true);
		demoCard.CardData.UpdateCardText();
		string dropSummaryFromCard = GetDropSummaryFromCard(cardData);
		string description = demoCard.CardData.Description;
		description = description.Replace("\\d", "\n\n");
		if (cardData is Combatable combatable)
		{
			description += combatable.GetCombatableDescriptionAdvanced();
		}
		if (!string.IsNullOrEmpty(dropSummaryFromCard) && cardData.MyCardType != CardType.Locations)
		{
			description = description + "\n\n" + dropSummaryFromCard;
		}
		if (cardData is Blueprint blueprint)
		{
			description = blueprint.GetText();
		}
		CardDescription.text = description;
	}
}
