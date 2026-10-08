using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class CardData : MonoBehaviour, IGameCardOrCardData
{
	[Header("General")]
	public string Id = "";

	[ExtraData("custom_name")]
	[HideInInspector]
	public string CustomName;

	[NonSerialized]
	[HideInInspector]
	public string descriptionOverride;

	[NonSerialized]
	[HideInInspector]
	public string nameOverride;

	[Term]
	public string NameTerm = "";

	[Term]
	public string DescriptionTerm = "";

	public PickupSoundGroup PickupSoundGroup;

	public AudioClip PickupSound;

	[HideInInspector]
	public string UniqueId = "";

	[HideInInspector]
	public string ParentUniqueId = "";

	[HideInInspector]
	public string EquipmentHolderUniqueId = "";

	[HideInInspector]
	public string WorkerHolderUniqueId = "";

	[HideInInspector]
	public int WorkerIndex = -1;

	public int Value = 1;

	public Sprite Icon;

	[HideInInspector]
	public GameCard MyGameCard;

	[HideInInspector]
	public bool IsFoil;

	public bool IsShiny;

	public CardType MyCardType;

	public bool IsBuilding;

	public bool IsCookedFood;

	public bool HideFromCardopedia;

	[HideInInspector]
	public List<StatusEffect> StatusEffects = new List<StatusEffect>();

	[HideInInspector]
	[ExtraData("creation_month")]
	public int CreationMonth;

	[HideInInspector]
	public CardUpdateType CardUpdateType;

	[HideInInspector]
	public float ExpectedValue;

	public bool HasUniquePalette;

	public CardPalette MyPalette;

	public List<ExtraCardData> LeftoverExtraData = new List<ExtraCardData>();

	internal string _name;

	private string _oldNameTerm;

	private string _cachedConnectorString;

	[Header("Cities options")]
	public int CitiesValue = 10;

	public int WorkerAmount;

	public bool EducatedWorkers;

	[HideInInspector]
	[ExtraData("output_direction")]
	public Vector3 OutputDir = Vector3.right;

	[SerializeReference]
	public List<RequirementHolder> RequirementHolders = new List<RequirementHolder>();

	public MonthlyRequirementResult MonthlyRequirementResult;

	[HideInInspector]
	public bool IsDamaged;

	[HideInInspector]
	public CardDamageType DamageType;

	[HideInInspector]
	[ExtraData("is_on")]
	public bool IsOn = true;

	[Header("Energy options")]
	public List<CardConnectorData> EnergyConnectors = new List<CardConnectorData>();

	private Dictionary<string, string> methodToActionId = new Dictionary<string, string>();

	private static List<string> tmpList = new List<string>();

	private static List<string> reqList = new List<string>();

	public string Name
	{
		get
		{
			if (!string.IsNullOrEmpty(nameOverride))
			{
				return nameOverride;
			}
			if (_oldNameTerm == NameTerm && !string.IsNullOrEmpty(_name))
			{
				return _name;
			}
			string text = SokLoc.Translate(NameTerm);
			if (MyCardType == CardType.Ideas)
			{
				text = ((!(this is Blueprint blueprint)) ? SokLoc.Translate("label_idea_fullname", LocParam.Create("name", text)) : (blueprint.IsInvention ? SokLoc.Translate("label_invention_fullname", LocParam.Create("name", text)) : ((!blueprint.IsLandmark) ? SokLoc.Translate("label_idea_fullname", LocParam.Create("name", text)) : SokLoc.Translate("label_landmark_fullname", LocParam.Create("name", text)))));
			}
			if (MyCardType == CardType.Rumors)
			{
				text = SokLoc.Translate("label_rumor_full", LocParam.Create("name", text));
			}
			_oldNameTerm = NameTerm;
			_name = text;
			return text;
		}
	}

	public Vector3 Position => MyGameCard.transform.position;

	public string FullName
	{
		get
		{
			string text = Name;
			if (IsFoil)
			{
				return text + " " + SokLoc.Translate("label_foil");
			}
			return text;
		}
	}

	public string Description
	{
		get
		{
			if (!string.IsNullOrEmpty(descriptionOverride))
			{
				return descriptionOverride;
			}
			return SokLoc.Translate(DescriptionTerm);
		}
	}

	public virtual bool DetermineCanHaveCardsWhenIsRoot => false;

	public virtual bool HasInventory => false;

	public virtual bool CanBeDragged => true;

	public List<CardBag> GetCardBags()
	{
		Type type = GetType();
		List<CardBag> list = new List<CardBag>();
		foreach (FieldInfo item2 in from x in type.GetFields()
			where x.FieldType == typeof(CardBag) || x.FieldType.IsSubclassOf(typeof(CardBag))
			select x)
		{
			CardBag item = (CardBag)item2.GetValue(this);
			list.Add(item);
		}
		foreach (FieldInfo item3 in type.GetFields().Where(delegate(FieldInfo x)
		{
			Type fieldType = x.FieldType;
			if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
			{
				Type type2 = fieldType.GetGenericArguments()[0];
				if (type2 == typeof(CardBag) || type2.IsSubclassOf(typeof(CardBag)))
				{
					return true;
				}
			}
			return false;
		}))
		{
			List<object> source = ((IEnumerable)item3.GetValue(this)).Cast<object>().ToList();
			list.AddRange(source.Cast<CardBag>());
		}
		return list;
	}

	public int GetValue()
	{
		if (MonthlyRequirementResult != null)
		{
			return -1;
		}
		if (MyGameCard != null && MyGameCard.IsDemoCard)
		{
			if (CardUpdateType != CardUpdateType.Cities)
			{
				return Value;
			}
			return CitiesValue;
		}
		WorldManager instance = WorldManager.instance;
		if ((object)instance == null || instance.CurrentBoard?.Location != Location.Cities)
		{
			return Value;
		}
		return CitiesValue;
	}

	public virtual bool ShouldCompleteTimer(string timerActionId)
	{
		return true;
	}

	public virtual bool ShouldStartTimerWorkers(string timerActionId)
	{
		if (WorkerAmount <= 0)
		{
			return true;
		}
		bool flag = WorkerAmountMet();
		if (EducatedWorkers && flag && MyGameCard.WorkerChildren.Any((GameCard c) => c.CardData.Id != "educated_worker" && c.CardData.Id != "genius" && c.CardData.Id != "robot_genius" && c.CardData.Id != "robot_worker"))
		{
			return false;
		}
		return flag;
	}

	public virtual bool ShouldStartTimerEnergy(CardData consumer, string timerActionId)
	{
		if (consumer == null)
		{
			return true;
		}
		if (consumer != null && consumer.HasEnergyInput())
		{
			return true;
		}
		return false;
	}

	public virtual void OnLanguageChange()
	{
		_name = null;
		_oldNameTerm = null;
		if (WorldManager.instance.GameDataLoader.ProfanityChecker.IsProfanityInLanguage(SokLoc.instance.CurrentLanguage, CustomName))
		{
			CustomName = "Bobba";
		}
		_cachedConnectorString = null;
	}

	protected virtual void OnValidate()
	{
		foreach (CardBag cardBag in GetCardBags())
		{
			cardBag.RecalculateOdds();
		}
	}

	public virtual bool HasEnergyOutput(CardConnector connectedNode = null, List<CardConnector> nodeTracker = null)
	{
		return false;
	}

	private bool HasEnergyInputConnector()
	{
		for (int i = 0; i < MyGameCard.CardConnectorChildren.Count; i++)
		{
			if (MyGameCard.CardConnectorChildren[i].CardDirection == CardDirection.input && MyGameCard.CardConnectorChildren[i].IsEnergyConnector)
			{
				return true;
			}
		}
		return false;
	}

	private bool AllInputConnectorsPowered()
	{
		for (int i = 0; i < MyGameCard.CardConnectorChildren.Count; i++)
		{
			if (MyGameCard.CardConnectorChildren[i].CardDirection == CardDirection.input && MyGameCard.CardConnectorChildren[i].IsEnergyConnector)
			{
				if (MyGameCard.CardConnectorChildren[i].ConnectedNode == null)
				{
					return false;
				}
				if (!MyGameCard.CardConnectorChildren[i].ConnectedNode.HasEnergyOutput())
				{
					return false;
				}
			}
		}
		return true;
	}

	private bool HasAnySewerOutput()
	{
		foreach (CardConnectorData energyConnector in EnergyConnectors)
		{
			if (energyConnector.EnergyConnectionStrength == ConnectionType.Sewer && energyConnector.EnergyConnectionType == CardDirection.output)
			{
				return true;
			}
		}
		return false;
	}

	private bool AllSewerOutputsFunctional()
	{
		foreach (CardConnector cardConnectorChild in MyGameCard.CardConnectorChildren)
		{
			if (cardConnectorChild.ConnectionType == ConnectionType.Sewer && cardConnectorChild.CardDirection == CardDirection.output && (cardConnectorChild.ConnectedNode == null || cardConnectorChild.ConnectedNode.Parent.CardData.IsDamaged))
			{
				return false;
			}
		}
		return true;
	}

	public bool HasSewerConnected()
	{
		if (!HasAnySewerOutput())
		{
			return true;
		}
		if (!AllSewerOutputsFunctional())
		{
			return false;
		}
		return true;
	}

	public virtual bool HasEnergyInput(CardConnector connectedNode = null)
	{
		if (!HasEnergyInputConnector())
		{
			return true;
		}
		if (AllInputConnectorsPowered())
		{
			return true;
		}
		return false;
	}

	public void NotifyEnergyConsumers()
	{
		foreach (CardConnector cardConnectorChild in MyGameCard.CardConnectorChildren)
		{
			if (cardConnectorChild.ConnectedNode != null)
			{
				cardConnectorChild.ConnectedNode.Parent.StackUpdate = true;
			}
		}
		MyGameCard.StackUpdate = true;
	}

	public string GetEnergyInputString()
	{
		return "todo: Implement this to show input amount";
	}

	public int GetRepairCost()
	{
		if (this is Creditcard)
		{
			return 20;
		}
		return Mathf.Max(10, Mathf.FloorToInt(CitiesValue / 10 / 2) * 10);
	}

	public virtual void OnSellCard()
	{
	}

	public virtual void OnInitialCreate()
	{
	}

	public bool CanHaveCardOnTop(CardData otherCard, bool isPrefab = false)
	{
		GameCard rootCard = MyGameCard.GetRootCard();
		int num = rootCard.GetChildCount() + 1;
		if (MyGameCard.CardData is Chest)
		{
			num = 0;
		}
		int num2 = (isPrefab ? 1 : (otherCard.MyGameCard.GetRootCard().GetChildCount() + 1));
		if (num + num2 > 30)
		{
			return false;
		}
		if (IsDamaged || rootCard.CardData.IsDamaged)
		{
			if (DamageType == CardDamageType.Fire || rootCard.CardData.DamageType == CardDamageType.Fire || DamageType == CardDamageType.Drought || rootCard.CardData.DamageType == CardDamageType.Drought)
			{
				if (otherCard.Id == "water")
				{
					return true;
				}
				return false;
			}
			if (DamageType == CardDamageType.Damaged || rootCard.CardData.DamageType == CardDamageType.Damaged)
			{
				if (otherCard is ICurrency)
				{
					return true;
				}
				return false;
			}
		}
		if (rootCard.CardData is HeavyFoundation && rootCard.HasChild && rootCard.Child.CardData.DetermineCanHaveCardsWhenIsRoot)
		{
			return rootCard.Child.CardData.CanHaveCard(otherCard);
		}
		if (rootCard.CardData.WorkerAmount > 0)
		{
			if (rootCard.CardData.EducatedWorkers && (otherCard.Id == "genius" || otherCard.Id == "robot_genius" || otherCard.Id == "educated_worker" || otherCard.Id == "robot_worker"))
			{
				return true;
			}
			if (otherCard is Worker)
			{
				return true;
			}
		}
		if (rootCard.CardData.DetermineCanHaveCardsWhenIsRoot)
		{
			return rootCard.CardData.CanHaveCard(otherCard);
		}
		if (MyGameCard.IsEquipped || MyGameCard.IsWorking)
		{
			return false;
		}
		if (MyGameCard.InConflict)
		{
			bool flag = false;
			if (otherCard.Id == "bone" && Id == "wolf")
			{
				flag = true;
			}
			else if (otherCard.Id == "milk" && Id == "feral_cat")
			{
				flag = true;
			}
			else if (otherCard.Id == "parrot" && Id == "pirate")
			{
				flag = true;
			}
			if (!(otherCard is Equipable || otherCard is Combatable || flag))
			{
				return false;
			}
		}
		return CanHaveCard(otherCard);
	}

	protected virtual bool CanHaveCard(CardData otherCard)
	{
		return false;
	}

	protected virtual bool CanSelectOutput()
	{
		return false;
	}

	public bool CanSelectOutputDirection()
	{
		return CanSelectOutput();
	}

	public bool HasOutputConnector()
	{
		return MyGameCard.CardConnectorChildren.Any((CardConnector x) => x.ConnectionType == ConnectionType.Transport && x.CardDirection == CardDirection.output);
	}

	protected virtual bool CanToggleOnOff()
	{
		return false;
	}

	public bool CanToggleCardOnOff()
	{
		if (MyGameCard.IsDemoCard)
		{
			return false;
		}
		return CanToggleOnOff();
	}

	public void ToggleCardOnOff()
	{
		IsOn = !IsOn;
	}

	public virtual void SetFoil()
	{
		IsFoil = true;
		if (Value != -1)
		{
			Value *= 5;
		}
		if (CitiesValue != -1)
		{
			CitiesValue *= 5;
		}
	}

	public virtual void Clicked()
	{
	}

	public bool HasCardOnTop<T>() where T : CardData
	{
		T card;
		return HasCardOnTop(out card);
	}

	public bool HasCardOnTop(string id)
	{
		if (!MyGameCard.HasChild)
		{
			return false;
		}
		return MyGameCard.Child.CardData.Id == id;
	}

	public virtual bool CanBePushedBy(CardData otherCard)
	{
		return true;
	}

	public bool HasCardOnTop(string id, out CardData cardData)
	{
		cardData = null;
		if (!MyGameCard.HasChild)
		{
			return false;
		}
		if (MyGameCard.Child.CardData.Id == id)
		{
			cardData = MyGameCard.Child.CardData;
			return true;
		}
		return false;
	}

	public bool HasCardOnTop<T>(out T card) where T : CardData
	{
		card = null;
		if (!MyGameCard.HasChild)
		{
			return false;
		}
		card = MyGameCard.Child.CardData as T;
		return card != null;
	}

	public bool IsOnCard<T>(out T card) where T : CardData
	{
		card = null;
		if (!MyGameCard.HasParent)
		{
			return false;
		}
		card = MyGameCard.Parent.CardData as T;
		return card != null;
	}

	protected virtual string GetTooltipText()
	{
		return $"{Name} (${GetValue()})\n<size=70%><i>{Description}</i></size>";
	}

	public virtual bool CanHaveCardsWhileHasStatus()
	{
		return false;
	}

	protected virtual void Awake()
	{
	}

	public virtual void UpdateCardText()
	{
		GameCard myGameCard = MyGameCard;
		if ((object)myGameCard != null && myGameCard.CardConnectorChildren.Count > 0 && MyGameCard.IsHovered)
		{
			descriptionOverride = SokLoc.Translate(DescriptionTerm);
			descriptionOverride = descriptionOverride + "\n\n<i>" + GetConnectorInfoString(MyGameCard) + "</i>";
		}
	}

	private int GetConnectorCount(GameCard card, CardDirection connectionType, ConnectionType strength)
	{
		int num = 0;
		for (int i = 0; i < card.CardConnectorChildren.Count; i++)
		{
			CardConnector cardConnector = card.CardConnectorChildren[i];
			if (cardConnector.CardDirection == connectionType && cardConnector.ConnectionType == strength)
			{
				num++;
			}
		}
		return num;
	}

	public string GetConnectorInfoString(GameCard card)
	{
		if (_cachedConnectorString != null)
		{
			return _cachedConnectorString;
		}
		string text = "";
		if (card != null && card.CardConnectorChildren.Count > 0)
		{
			int connectorCount = GetConnectorCount(card, CardDirection.input, ConnectionType.LV);
			int connectorCount2 = GetConnectorCount(card, CardDirection.input, ConnectionType.HV);
			if (connectorCount > 0 || connectorCount2 > 0)
			{
				text += SokLoc.Translate("label_energy_input");
				if (connectorCount > 0)
				{
					text += $" {connectorCount}{Icons.LV}";
				}
				if (connectorCount2 > 0)
				{
					text += $" {connectorCount2}{Icons.HV}";
				}
			}
			int connectorCount3 = GetConnectorCount(card, CardDirection.output, ConnectionType.LV);
			int connectorCount4 = GetConnectorCount(card, CardDirection.output, ConnectionType.HV);
			if (connectorCount3 > 0 || connectorCount4 > 0)
			{
				if (!string.IsNullOrEmpty(text))
				{
					text += "\n\n";
				}
				text += SokLoc.Translate("label_energy_output");
				if (connectorCount3 > 0)
				{
					text += $" {connectorCount3}{Icons.LV}";
				}
				if (connectorCount4 > 0)
				{
					text += $" {connectorCount4}{Icons.HV}";
				}
			}
		}
		_cachedConnectorString = text;
		return text;
	}

	public virtual void UpdateCard()
	{
		if (MyGameCard.IsDemoCard || !MyGameCard.MyBoard.IsCurrent)
		{
			return;
		}
		MyGameCard.HighlightActive = false;
		if (WorldManager.instance.DraggingCard != null && WorldManager.instance.DraggingCard != MyGameCard)
		{
			if (CanHaveCardOnTop(WorldManager.instance.DraggingCard.CardData) && !MyGameCard.HasChild && !MyGameCard.IsChildOf(WorldManager.instance.DraggingCard))
			{
				MyGameCard.HighlightActive = true;
			}
			if (!(MyGameCard.removedChild == WorldManager.instance.DraggingCard))
			{
				GameCard cardWithStatusInStack = MyGameCard.GetCardWithStatusInStack();
				if (cardWithStatusInStack != null && !cardWithStatusInStack.CardData.CanHaveCardsWhileHasStatus())
				{
					MyGameCard.HighlightActive = false;
				}
			}
		}
		if (MyGameCard.StackUpdate)
		{
			if (HasStatusEffectOfType<StatusEffect_MaxOnBoard>())
			{
				RemoveStatusEffect<StatusEffect_MaxOnBoard>();
			}
			if (!WorkerAmountMet() && MyGameCard.TimerRunning && !MyGameCard.SkipCitiesChecks)
			{
				MyGameCard.CancelAnyTimer();
			}
			CheckBlueprintInStack();
		}
		if (!MyGameCard.BeingDragged && MyGameCard.LastParent != null && !MyGameCard.HasParent)
		{
			if (MyGameCard.LastParent.GetRootCard().CardData.DetermineCanHaveCardsWhenIsRoot)
			{
				CheckStackValidityAndRestack();
			}
			MyGameCard.LastParent = null;
		}
		if (WorkerAmount > 0)
		{
			bool flag = WorkerAmountMet();
			if (EducatedWorkers)
			{
				if (MyGameCard.WorkerChildren.Any((GameCard c) => c.CardData is Worker worker && worker.GetWorkerType() != WorkerType.Educated && worker.GetWorkerType() != WorkerType.Robot) || !flag)
				{
					if (!HasStatusEffectOfType<StatusEffect_NoEducatedWorkers>())
					{
						AddStatusEffect(new StatusEffect_NoEducatedWorkers());
					}
				}
				else
				{
					RemoveStatusEffect<StatusEffect_NoEducatedWorkers>();
				}
			}
			else if (!flag)
			{
				if (!HasStatusEffectOfType<StatusEffect_NoWorkers>())
				{
					AddStatusEffect(new StatusEffect_NoWorkers());
				}
			}
			else
			{
				RemoveStatusEffect<StatusEffect_NoWorkers>();
			}
			for (int i = 0; i < MyGameCard.WorkerChildren.Count; i++)
			{
				GameCard gameCard = MyGameCard.WorkerChildren[i];
				if (gameCard.HasParent || gameCard.HasChild)
				{
					gameCard.RemoveFromStack();
				}
			}
		}
		else
		{
			if (HasStatusEffectOfType<StatusEffect_NoWorkers>())
			{
				RemoveStatusEffect<StatusEffect_NoWorkers>();
			}
			if (HasStatusEffectOfType<StatusEffect_NoEducatedWorkers>())
			{
				RemoveStatusEffect<StatusEffect_NoEducatedWorkers>();
			}
		}
		if (!IsOn && ((WorkerAmount > 0 && WorkerAmountMet()) || WorkerAmount == 0))
		{
			AddStatusEffect(new StatusEffect_CardOff());
		}
		else
		{
			RemoveStatusEffect<StatusEffect_CardOff>();
		}
		for (int num = StatusEffects.Count - 1; num >= 0; num--)
		{
			StatusEffects[num].Update();
		}
		UpdateCardText();
	}

	private void CheckBlueprintInStack()
	{
		if (!MyGameCard.HasParent)
		{
			Subprint subprint = FindMatchingPrint();
			if (subprint != null)
			{
				string id = subprint.ParentBlueprint.Id;
				int subprintIndex = subprint.SubprintIndex;
				BaseVillager baseVillager = (from BaseVillager x in CardsInStackMatchingPredicate((CardData x) => x is BaseVillager)
					orderby x.GetActionTimeModifier("finish_blueprint", this)
					select x).Reverse().FirstOrDefault();
				Worker worker = (from Worker x in CardsInStackMatchingPredicate((CardData x) => x is Worker)
					orderby x.GetActionTimeModifier()
					select x).Reverse().FirstOrDefault();
				if (!subprint.ParentBlueprint.IsInvention || (subprint.ParentBlueprint.IsInvention && WorldManager.instance.HasFoundCard(id)))
				{
					CardData consumer = CardsInStackMatchingPredicate((CardData x) => x is IEnergyConsumer).FirstOrDefault();
					if (baseVillager != null)
					{
						MyGameCard.StartBlueprintTimer(baseVillager.GetActionTimeModifier("finish_blueprint", this) * subprint.Time, FinishBlueprint, subprint.StatusName, GetActionId("FinishBlueprint"), id, subprintIndex, consumer, subprint.ParentBlueprint.IgnoreEnergyWorkerDemand);
					}
					else if (worker != null)
					{
						MyGameCard.StartBlueprintTimer(worker.GetActionTimeModifier() * subprint.Time, FinishBlueprint, subprint.StatusName, GetActionId("FinishBlueprint"), id, subprintIndex, consumer, subprint.ParentBlueprint.IgnoreEnergyWorkerDemand);
					}
					else
					{
						MyGameCard.StartBlueprintTimer(subprint.Time, FinishBlueprint, subprint.StatusName, GetActionId("FinishBlueprint"), id, subprintIndex, consumer, subprint.ParentBlueprint.IgnoreEnergyWorkerDemand);
					}
				}
			}
			else
			{
				MyGameCard.CancelTimer(GetActionId("FinishBlueprint"));
			}
		}
		else
		{
			MyGameCard.CancelTimer(GetActionId("FinishBlueprint"));
		}
		if (MyGameCard.TimerRunning && MyGameCard.TimerActionId == "finish_blueprint" && FindMatchingPrint() == null)
		{
			MyGameCard.CancelTimer(GetActionId("FinishBlueprint"));
		}
		MyGameCard.StackUpdate = false;
	}

	private void CheckStackValidityAndRestack()
	{
		List<GameCard> allCardsInStack = MyGameCard.GetAllCardsInStack();
		List<GameCard> list = new List<GameCard>();
		for (int i = 0; i < allCardsInStack.Count; i++)
		{
			list.Add(allCardsInStack[i]);
			allCardsInStack[i].RemoveFromStack();
			if (i < allCardsInStack.Count - 1 && !allCardsInStack[i].CardData.CanHaveCardOnTop(allCardsInStack[i + 1].CardData))
			{
				WorldManager.instance.Restack(list);
				list.Clear();
			}
		}
		WorldManager.instance.Restack(list);
	}

	public virtual void ParseAction(string actions)
	{
		string[] array = actions.Split(';');
		foreach (string text in array)
		{
			if (string.IsNullOrWhiteSpace(text))
			{
				break;
			}
			if (text.StartsWith("add"))
			{
				string statusEffect = text.Split(':')[1];
				StatusEffect statusEffectWithName = GetStatusEffectWithName(statusEffect);
				if (HasStatusEffectOfType(statusEffectWithName))
				{
					RemoveStatusEffect(statusEffectWithName.GetType());
				}
				AddStatusEffect(statusEffectWithName);
			}
			if (text.StartsWith("remove"))
			{
				string statusEffect2 = text.Split(':')[1];
				RemoveStatusEffect(statusEffect2);
			}
			if (text.StartsWith("create"))
			{
				string cardId = text.Split(':')[1];
				WorldManager.instance.CreateCard(base.transform.position, cardId, faceUp: false, checkAddToStack: false);
			}
			if (text.StartsWith("special"))
			{
				string action = text.Split(':')[1];
				QuestManager.instance.SpecialActionComplete(action);
			}
		}
	}

	private Subprint FindMatchingPrint()
	{
		Subprint result = null;
		int num = int.MaxValue;
		int num2 = int.MinValue;
		foreach (Blueprint blueprintPrefab in WorldManager.instance.BlueprintPrefabs)
		{
			if (!blueprintPrefab.CanCurrentlyBeMade || (WorldManager.instance.CurrentBoard.Location == Location.Cities && blueprintPrefab.CardUpdateType != CardUpdateType.Cities))
			{
				continue;
			}
			SubprintMatchInfo matchInfo;
			Subprint matchingSubprint = blueprintPrefab.GetMatchingSubprint(MyGameCard.GetRootCard(), out matchInfo);
			if (matchingSubprint == null)
			{
				continue;
			}
			if (blueprintPrefab.HasMaxAmountOnBoard && WorldManager.instance.GetCardCount(matchingSubprint.ResultCard) >= blueprintPrefab.MaxAmountOnBoard)
			{
				if (!HasStatusEffectOfType<StatusEffect_MaxOnBoard>())
				{
					AddStatusEffect(new StatusEffect_MaxOnBoard());
				}
			}
			else if (matchInfo.MatchCount > num2 || (matchInfo.MatchCount == num2 && matchInfo.FullyMatchedAt < num))
			{
				num = matchInfo.FullyMatchedAt;
				num2 = matchInfo.MatchCount;
				result = matchingSubprint;
			}
		}
		return result;
	}

	public void EquipItem(Equipable equipable)
	{
		Equipable equipableOfEquipableType = GetEquipableOfEquipableType(equipable.EquipableType);
		if (equipableOfEquipableType != null)
		{
			MyGameCard.Unequip(equipableOfEquipableType);
			equipableOfEquipableType.MyGameCard.SendIt();
		}
		MyGameCard.Equip(equipable);
		if (!(this is Mob))
		{
			QuestManager.instance.ActionComplete(MyGameCard.CardData, "equip_item");
		}
	}

	public void EquipWorker(Worker worker)
	{
		List<int> second = MyGameCard.WorkerChildren.Select((GameCard x) => x.CardData.WorkerIndex).ToList();
		List<int> list = new List<int>();
		for (int i = 0; i < WorkerAmount; i++)
		{
			list.Add(i);
		}
		int index = list.Except(second).DefaultIfEmpty(WorkerAmount - 1).First();
		if (MyGameCard.WorkerChildren.Count > index)
		{
			GameCard gameCard = MyGameCard.WorkerChildren.Where((GameCard x) => x.CardData.WorkerIndex == index).FirstOrDefault();
			if (gameCard != null)
			{
				MyGameCard.UnequipWorker(gameCard);
				gameCard.SendIt();
			}
		}
		MyGameCard.EquipWorker(worker, index);
	}

	public virtual void StoppedDragging()
	{
	}

	public virtual void OnEquipItem(Equipable equipable)
	{
	}

	public virtual void OnUnequipItem(Equipable equipable)
	{
	}

	public virtual void OnDestroyCard()
	{
	}

	[TimedAction("finish_blueprint")]
	public void FinishBlueprint()
	{
		Blueprint blueprintWithId = WorldManager.instance.GetBlueprintWithId(MyGameCard.TimerBlueprintId);
		if (blueprintWithId != null)
		{
			blueprintWithId.BlueprintComplete(MyGameCard, MyGameCard.GetAllCardsInStack(), blueprintWithId.Subprints[MyGameCard.TimerSubprintIndex]);
		}
	}

	public List<ExtraCardData> GetExtraCardData()
	{
		return GetExtraCardData(this);
	}

	public static List<ExtraCardData> GetExtraCardData(object o)
	{
		List<ExtraCardData> list = new List<ExtraCardData>();
		FieldInfo[] fields = o.GetType().GetFields();
		foreach (FieldInfo fieldInfo in fields)
		{
			ExtraDataAttribute[] array = (ExtraDataAttribute[])fieldInfo.GetCustomAttributes(typeof(ExtraDataAttribute), inherit: true);
			foreach (ExtraDataAttribute extraDataAttribute in array)
			{
				if (fieldInfo.FieldType == typeof(string))
				{
					list.Add(new ExtraCardData(extraDataAttribute.Identifier, (string)fieldInfo.GetValue(o)));
				}
				else if (fieldInfo.FieldType == typeof(int))
				{
					list.Add(new ExtraCardData(extraDataAttribute.Identifier, (int)fieldInfo.GetValue(o)));
				}
				else if (fieldInfo.FieldType == typeof(float))
				{
					list.Add(new ExtraCardData(extraDataAttribute.Identifier, (float)fieldInfo.GetValue(o)));
				}
				else if (fieldInfo.FieldType == typeof(Vector3))
				{
					list.Add(new ExtraCardData(extraDataAttribute.Identifier, (Vector3)fieldInfo.GetValue(o)));
				}
				else if (fieldInfo.FieldType == typeof(bool))
				{
					list.Add(new ExtraCardData(extraDataAttribute.Identifier, (bool)fieldInfo.GetValue(o)));
				}
				else
				{
					Debug.LogError("Can't serialize field " + fieldInfo.Name + " with ExtraDataAttribute because it's not an int or a string!");
				}
			}
		}
		if (o is CardData cardData)
		{
			list.AddRange(cardData.LeftoverExtraData);
		}
		return list;
	}

	public void SetExtraCardData(List<ExtraCardData> extraData)
	{
		SetExtraCardData(this, extraData);
	}

	public static void SetExtraCardData(object o, List<ExtraCardData> extraData)
	{
		FieldInfo[] fields = o.GetType().GetFields();
		foreach (FieldInfo fieldInfo in fields)
		{
			ExtraDataAttribute[] array = (ExtraDataAttribute[])fieldInfo.GetCustomAttributes(typeof(ExtraDataAttribute), inherit: true);
			foreach (ExtraDataAttribute attribute in array)
			{
				ExtraCardData extraCardData = extraData.FirstOrDefault((ExtraCardData x) => x.AttributeId == attribute.Identifier);
				if (extraCardData != null)
				{
					if (fieldInfo.FieldType == typeof(string))
					{
						fieldInfo.SetValue(o, extraCardData.StringValue);
					}
					else if (fieldInfo.FieldType == typeof(int))
					{
						fieldInfo.SetValue(o, extraCardData.IntValue);
					}
					else if (fieldInfo.FieldType == typeof(float))
					{
						fieldInfo.SetValue(o, extraCardData.FloatValue);
					}
					else if (fieldInfo.FieldType == typeof(Vector3))
					{
						fieldInfo.SetValue(o, extraCardData.VectorValue);
					}
					else if (fieldInfo.FieldType == typeof(bool))
					{
						fieldInfo.SetValue(o, extraCardData.BoolValue);
					}
					else
					{
						Debug.LogError("Can't deserialize field " + fieldInfo.Name + " with ExtraDataAttribute because it's not an int or a string!");
					}
					extraData.Remove(extraCardData);
				}
				else
				{
					Debug.LogWarning($"Could not find matching data for {fieldInfo.Name} in {o.GetType()}, using default value..");
				}
			}
		}
		if (o is CardData cardData)
		{
			cardData.LeftoverExtraData = extraData;
		}
	}

	public string GetActionId(string methodName)
	{
		if (!methodToActionId.ContainsKey(methodName))
		{
			TimedActionAttribute[] array = (TimedActionAttribute[])GetType().GetMethod(methodName).GetCustomAttributes(typeof(TimedActionAttribute), inherit: true);
			methodToActionId[methodName] = array[0].Identifier;
		}
		return methodToActionId[methodName];
	}

	public TimerAction GetDelegateForActionId(string id)
	{
		MethodInfo[] methods = GetType().GetMethods();
		foreach (MethodInfo methodInfo in methods)
		{
			TimedActionAttribute[] array = (TimedActionAttribute[])methodInfo.GetCustomAttributes(typeof(TimedActionAttribute), inherit: true);
			if (array.Length != 0 && array[0].Identifier == id)
			{
				return (TimerAction)methodInfo.CreateDelegate(typeof(TimerAction), this);
			}
		}
		Debug.LogError("Could not find delegate for id " + id);
		return null;
	}

	public bool AllChildrenMatchPredicate(Predicate<CardData> pred)
	{
		if (MyGameCard == null)
		{
			return false;
		}
		GameCard child = MyGameCard.Child;
		while (child != null)
		{
			if (!pred(child.CardData))
			{
				return false;
			}
			child = child.Child;
		}
		return true;
	}

	public bool AnyChildMatchesPredicate(Predicate<CardData> pred)
	{
		CardData match;
		return AnyChildMatchesPredicate(pred, out match);
	}

	public bool AnyChildMatchesPredicate(Predicate<CardData> pred, out CardData match)
	{
		match = null;
		if (MyGameCard == null)
		{
			return false;
		}
		GameCard child = MyGameCard.Child;
		while (child != null)
		{
			if (pred(child.CardData))
			{
				match = child.CardData;
				return true;
			}
			child = child.Child;
		}
		return false;
	}

	protected int ChildrenMatchingPredicateCount(Predicate<CardData> pred)
	{
		int num = 0;
		if (MyGameCard == null)
		{
			return num;
		}
		GameCard child = MyGameCard.Child;
		while (child != null)
		{
			if (pred(child.CardData))
			{
				num++;
			}
			child = child.Child;
		}
		return num;
	}

	public List<CardData> CardsInStackMatchingPredicate(Predicate<CardData> pred)
	{
		List<CardData> list = new List<CardData>();
		GetCardsInStackMatchingPredicate(pred, list);
		return list;
	}

	public void GetCardsInStackMatchingPredicate(Predicate<CardData> pred, List<CardData> outList)
	{
		outList.Clear();
		if (MyGameCard == null)
		{
			if (pred(this))
			{
				outList.Add(this);
			}
			return;
		}
		GameCard gameCard = MyGameCard.GetRootCard();
		while (gameCard != null)
		{
			if (pred(gameCard.CardData))
			{
				outList.Add(gameCard.CardData);
			}
			gameCard = gameCard.Child;
		}
	}

	public List<CardData> ChildrenMatchingPredicate(Predicate<CardData> pred)
	{
		List<CardData> list = new List<CardData>();
		if (MyGameCard == null)
		{
			return list;
		}
		GameCard child = MyGameCard.Child;
		while (child != null)
		{
			if (pred(child.CardData))
			{
				list.Add(child.CardData);
			}
			child = child.Child;
		}
		return list;
	}

	public void GetChildrenMatchingPredicate(Predicate<CardData> pred, List<CardData> result)
	{
		result.Clear();
		if (MyGameCard == null)
		{
			return;
		}
		GameCard child = MyGameCard.Child;
		while (child != null)
		{
			if (pred(child.CardData))
			{
				result.Add(child.CardData);
			}
			child = child.Child;
		}
	}

	protected void RemoveFirstChildFromStack()
	{
		if (!(MyGameCard == null))
		{
			GameCard child = MyGameCard.Child;
			GameCard child2 = child.Child;
			child.RemoveFromStack();
			if (child2 != null)
			{
				child2.SetParent(MyGameCard);
			}
		}
	}

	public void RestackChildrenMatchingPredicate(Predicate<CardData> pred)
	{
		List<GameCard> list = new List<GameCard>();
		list.Add(MyGameCard);
		List<GameCard> list2 = new List<GameCard>();
		List<GameCard> childCards = MyGameCard.GetChildCards();
		GameCard child = MyGameCard.Child;
		while (child != null)
		{
			if (pred(child.CardData))
			{
				list2.Add(child);
			}
			else
			{
				list.Add(child);
			}
			child = child.Child;
		}
		foreach (GameCard item in childCards)
		{
			item.RemoveFromStack();
		}
		WorldManager.instance.Restack(list);
		WorldManager.instance.Restack(list2);
	}

	public void DestroyChildrenMatchingPredicateAndRestack(Predicate<CardData> pred, int count)
	{
		GameCard parent = MyGameCard.Parent;
		List<GameCard> list = new List<GameCard>();
		list.Add(MyGameCard);
		List<GameCard> list2 = new List<GameCard>();
		GameCard child = MyGameCard.Child;
		while (child != null)
		{
			if (pred(child.CardData))
			{
				if (count > 0)
				{
					list2.Add(child);
					count--;
				}
				else
				{
					list.Add(child);
				}
			}
			else
			{
				list.Add(child);
			}
			child = child.Child;
		}
		foreach (GameCard item in list2)
		{
			item.DestroyCard(spawnSmoke: true, playSound: false);
		}
		WorldManager.instance.Restack(list);
		MyGameCard.Parent = parent;
	}

	public void AddStatusEffect(StatusEffect effect)
	{
		if (!HasStatusEffectOfType(effect) && !MyGameCard.IsDemoCard)
		{
			StatusEffects.Add(effect);
			effect.ParentCard = this;
			MyGameCard.StatusEffectsChanged();
			if (this is BaseVillager)
			{
				QuestManager.instance.SpecialActionComplete($"add_status_{effect}");
			}
		}
	}

	public void RemoveStatusEffect(StatusEffect effect)
	{
		if (StatusEffects.RemoveAll((StatusEffect x) => x == effect) != 0)
		{
			MyGameCard.StatusEffectsChanged();
			QuestManager.instance.SpecialActionComplete($"remove_status_{effect}");
		}
	}

	public void RemoveStatusEffect<T>() where T : StatusEffect
	{
		if (StatusEffects.RemoveAll((StatusEffect x) => x.GetType() == typeof(T)) != 0)
		{
			MyGameCard.StatusEffectsChanged();
			QuestManager.instance.SpecialActionComplete("remove_status_" + typeof(T).Name);
		}
	}

	public void RemoveStatusEffect(Type t)
	{
		if (StatusEffects.RemoveAll((StatusEffect x) => x.GetType() == t) != 0)
		{
			MyGameCard.StatusEffectsChanged();
			QuestManager.instance.SpecialActionComplete("remove_status_" + t.Name);
		}
	}

	public void RemoveAllStatusEffects()
	{
		StatusEffects.Clear();
		MyGameCard.StatusEffectsChanged();
	}

	public List<string> GetPossibleDrops()
	{
		if (this is FishingSpot fishingSpot)
		{
			List<string> cardsInBag = fishingSpot.NormalCardBag.GetCardsInBag();
			cardsInBag.AddRange(fishingSpot.FisherCardBag.GetCardsInBag());
			return cardsInBag;
		}
		if (this is Harvestable harvestable)
		{
			return harvestable.MyCardBag.GetCardsInBag();
		}
		if (this is Enemy enemy)
		{
			List<string> cardsInBag2 = enemy.Drops.GetCardsInBag();
			if (enemy.CanHaveInventory)
			{
				cardsInBag2.AddRange(enemy.PossibleEquipables.Select((Equipable x) => x.Id).ToList());
			}
			return cardsInBag2;
		}
		return new List<string>();
	}

	public bool HasUndiscoveredCardInDrops()
	{
		foreach (string item in GetPossibleDrops().Distinct().ToList())
		{
			if (!WorldManager.instance.CurrentSave.FoundCardIds.Contains(item))
			{
				return true;
			}
		}
		return false;
	}

	public void RemoveStatusEffect(string statusEffect)
	{
		if (!StatusEffect.StatusEffectExists(statusEffect))
		{
			Debug.LogError("Status effect with name " + statusEffect + " does not exist");
			return;
		}
		QuestManager.instance.SpecialActionComplete("remove_status_" + statusEffect);
		if (StatusEffects.RemoveAll((StatusEffect x) => x.GetType().Name == statusEffect) > 0)
		{
			MyGameCard.StatusEffectsChanged();
		}
	}

	public StatusEffect GetStatusEffectWithName(string statusEffect)
	{
		if (!StatusEffect.StatusEffectExists(statusEffect))
		{
			Debug.LogError("Status effect with name " + statusEffect + " does not exist");
			return null;
		}
		return StatusEffect.CreateStatusEffectFromName(statusEffect);
	}

	public void AddStatusEffect(string statusEffect)
	{
		if (!StatusEffect.StatusEffectExists(statusEffect))
		{
			Debug.LogError("Status effect with name " + statusEffect + " does not exist");
			return;
		}
		StatusEffect effect = StatusEffect.CreateStatusEffectFromName(statusEffect);
		AddStatusEffect(effect);
	}

	public bool HasStatusEffectOfType<T>() where T : StatusEffect
	{
		for (int i = 0; i < StatusEffects.Count; i++)
		{
			if (StatusEffects[i].GetType() == typeof(T))
			{
				return true;
			}
		}
		return false;
	}

	public bool HasAnyStatusEffect()
	{
		return StatusEffects.Count > 0;
	}

	public bool HasStatusEffectOfType(StatusEffect effect)
	{
		for (int i = 0; i < StatusEffects.Count; i++)
		{
			if (StatusEffects[i].GetType() == effect.GetType())
			{
				return true;
			}
		}
		return false;
	}

	public List<Equipable> GetAllEquipables()
	{
		List<Equipable> list = new List<Equipable>();
		if (MyGameCard == null)
		{
			return list;
		}
		foreach (GameCard equipmentChild in MyGameCard.EquipmentChildren)
		{
			if (equipmentChild.CardData is Equipable item)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public bool HasEquipableWithId(string id)
	{
		foreach (GameCard equipmentChild in MyGameCard.EquipmentChildren)
		{
			if (equipmentChild.CardData is Equipable equipable && equipable.Id == id)
			{
				return true;
			}
		}
		return false;
	}

	public bool HasEquipableOfType<T>() where T : Equipable
	{
		foreach (GameCard equipmentChild in MyGameCard.EquipmentChildren)
		{
			if (equipmentChild.CardData is Equipable equipable && equipable.GetType() == typeof(T))
			{
				return true;
			}
		}
		return false;
	}

	public bool HasEquipableOfEquipableType(EquipableType type)
	{
		foreach (GameCard equipmentChild in MyGameCard.EquipmentChildren)
		{
			if (equipmentChild.CardData is Equipable equipable && equipable.EquipableType == type)
			{
				return true;
			}
		}
		return false;
	}

	public Equipable GetEquipableOfEquipableType(EquipableType type)
	{
		foreach (GameCard equipmentChild in MyGameCard.EquipmentChildren)
		{
			if (equipmentChild.CardData is Equipable equipable && equipable.EquipableType == type)
			{
				return equipable;
			}
		}
		return null;
	}

	public bool WorkerAmountMet()
	{
		if (WorkerAmount <= 0)
		{
			return true;
		}
		int num = 0;
		for (int i = 0; i < MyGameCard.WorkerChildren.Count; i++)
		{
			if (MyGameCard.WorkerChildren[i] != null)
			{
				num++;
			}
		}
		return num >= WorkerAmount;
	}

	public static string CardToTermId(CardData card)
	{
		if (card is Blueprint)
		{
			string id = card.Id;
			id = id.Replace("blueprint_", "");
			return "idea_" + id;
		}
		return "card_" + card.Id;
	}

	public bool AllCardsInStackMatchPred(CardData card, Predicate<CardData> pred)
	{
		if (card.MyGameCard == null)
		{
			return pred(card);
		}
		return WorldManager.instance.AllCardsInStackMatchPred(card.MyGameCard, (GameCard x) => x.CardData is Combatable { CanAttack: not false } combatable && !(combatable is Animal));
	}

	public int GetChildCount()
	{
		if (MyGameCard == null)
		{
			return 0;
		}
		return MyGameCard.GetChildCount();
	}

	public void LogCardReferences()
	{
		List<ICardReference> list = (from x in ((WorldManager.instance != null) ? WorldManager.instance.GameDataLoader : new GameDataLoader()).DetermineCardReferences()
			where x.ReferencedCardId == Id
			select x).ToList();
		Debug.Log(string.Format("Referenced {0} times: {1}", list.Count, string.Join(", ", list)));
	}

	public void LogBlueprintUses()
	{
		List<Blueprint> list = ((WorldManager.instance != null) ? WorldManager.instance.GameDataLoader : new GameDataLoader()).BlueprintPrefabs.Where((Blueprint bp) => BlueprintUsesCard(bp, Id)).ToList();
		Debug.Log(string.Format("Used in blueprint {0} times:\n{1}", list.Count, string.Join("\n", list)));
	}

	private bool BlueprintUsesCard(Blueprint bp, string id)
	{
		for (int i = 0; i < bp.Subprints.Count; i++)
		{
			Subprint subprint = bp.Subprints[i];
			for (int j = 0; j < subprint.RequiredCards.Length; j++)
			{
				if (subprint.RequiredCards[j] == id)
				{
					return true;
				}
			}
		}
		return false;
	}

	public int GetDollarCountInStack(bool includeInChest)
	{
		if (includeInChest)
		{
			return ChildrenMatchingPredicate((CardData x) => x is ICurrency).Cast<ICurrency>().Sum((ICurrency x) => x.CurrencyValue);
		}
		return ChildrenMatchingPredicate((CardData x) => x is Dollar).Cast<Dollar>().Sum((Dollar x) => x.DollarValue);
	}

	public void UpdateRequirementResultsInStack(RequirementType requirementType, int add, GameCard card)
	{
		GameCard gameCard = ((MyGameCard.WorkerHolder != null) ? MyGameCard.WorkerHolder : MyGameCard.GetRootCard());
		string key = $"{card.CardData.Id}_{requirementType}";
		MonthlyRequirementResult monthlyRequirementResult = ((gameCard.CardData.MonthlyRequirementResult != null) ? gameCard.CardData.MonthlyRequirementResult : new MonthlyRequirementResult());
		if (monthlyRequirementResult.results.ContainsKey(key))
		{
			monthlyRequirementResult.results[key].Amount += add;
			if (monthlyRequirementResult.results[key].Card != card)
			{
				monthlyRequirementResult.results[key].CardAmount++;
			}
		}
		else
		{
			monthlyRequirementResult.results[key] = new MonthlyResult();
			monthlyRequirementResult.results[key].Amount = add;
			monthlyRequirementResult.results[key].CardAmount = 1;
			monthlyRequirementResult.results[key].Card = card;
			monthlyRequirementResult.results[key].Type = requirementType;
		}
		gameCard.CardData.MonthlyRequirementResult = monthlyRequirementResult;
	}

	public string GetRequirementDescription(GameCard card, int multipleAmount = 1, bool onlyShowCurrentlySatisfied = true)
	{
		reqList.Clear();
		foreach (RequirementHolder requirementHolder in RequirementHolders)
		{
			bool flag = true;
			bool flag2 = true;
			tmpList.Clear();
			foreach (CardRequirement cardRequirement in requirementHolder.CardRequirements)
			{
				string item = cardRequirement.RequirementDescriptionNeed(multipleAmount);
				tmpList.Add(item);
			}
			string text = string.Join("& ", tmpList);
			tmpList.Clear();
			foreach (CardRequirement cardRequirement2 in requirementHolder.CardRequirements)
			{
				string item2 = cardRequirement2.RequirementDescriptionNeedNegative(multipleAmount);
				tmpList.Add(item2);
			}
			string text2 = string.Join("& ", tmpList);
			tmpList.Clear();
			foreach (CardRequirementResult positiveResult in requirementHolder.PositiveResults)
			{
				string text3 = positiveResult.RequirementDescriptionPositive(multipleAmount, card);
				if (!string.IsNullOrEmpty(text3))
				{
					tmpList.Add(text3);
				}
			}
			string text4 = string.Join(", ", tmpList);
			tmpList.Clear();
			foreach (CardRequirementResult negativeResult in requirementHolder.NegativeResults)
			{
				string text5 = negativeResult.RequirementDescriptionNegative(multipleAmount, card);
				if (!string.IsNullOrEmpty(text5))
				{
					tmpList.Add(text5);
				}
			}
			string text6 = string.Join(", ", tmpList);
			if (onlyShowCurrentlySatisfied)
			{
				if (requirementHolder.CardRequirements.All((CardRequirement x) => x.Satisfied(card)))
				{
					flag2 = false;
				}
				else
				{
					flag = false;
				}
			}
			if (!string.IsNullOrEmpty(text4) && flag)
			{
				reqList.Add(text + "\u00a0" + text4);
			}
			if (!string.IsNullOrEmpty(text6) && flag2)
			{
				reqList.Add(text2 + "\u00a0" + text6);
			}
		}
		return string.Join("\n", reqList);
	}

	public void SetCardDamaged(CardDamageType type)
	{
		IsDamaged = true;
		DamageType = type;
		MyGameCard.UpdateCardPalette();
	}

	public void SetCardUndamaged()
	{
		IsDamaged = false;
		DamageType = CardDamageType.None;
		MyGameCard.UpdateCardPalette();
	}
}
