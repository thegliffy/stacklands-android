using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Harvestable : CardData
{
	[Header("Harvestable")]
	[Term]
	public string StatusTerm = "";

	[ExtraData("amount")]
	public int Amount = 3;

	public bool IsUnlimited;

	public float HarvestTime = 10f;

	public CardBag MyCardBag;

	[Header("Multiple villager options")]
	public int RequiredVillagerCount = 1;

	private List<CardData> villagers = new List<CardData>();

	[Card]
	public List<string> CanHaveCardIds = new List<string>();

	public string StatusText => SokLoc.Translate(StatusTerm);

	public override bool DetermineCanHaveCardsWhenIsRoot => true;

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (!(otherCard is BaseVillager) && !(otherCard is Worker) && !(otherCard.Id == Id) && (MyCardType != CardType.Weather || otherCard.MyCardType != CardType.Weather))
		{
			return CanHaveCardIds.Contains(otherCard.Id);
		}
		return true;
	}

	public override void SetFoil()
	{
		base.SetFoil();
	}

	protected override void Awake()
	{
		base.Awake();
		SokLoc.instance.LanguageChanged += UpdateDescription;
		UpdateDescription();
	}

	private void OnDestroy()
	{
		SokLoc.instance.LanguageChanged -= UpdateDescription;
	}

	private void UpdateDescription()
	{
		if (MyCardType == CardType.Locations)
		{
			descriptionOverride = SokLoc.Translate(DescriptionTerm, LocParam.Create("required_count", RequiredVillagerCount.ToString())) + "\n\n" + BoosterpackData.GetSummaryFromAllCards(MyCardBag.GetCardsInBag());
		}
	}

	public override void UpdateCard()
	{
		if (!(this is EnergyHarvestable))
		{
			GetChildrenMatchingPredicate((CardData x) => x is BaseVillager || x is Worker, villagers);
			bool flag = true;
			GameCard cardWithStatusInStack = MyGameCard.GetCardWithStatusInStack();
			if (cardWithStatusInStack != null && cardWithStatusInStack.TimerRunning && cardWithStatusInStack.TimerActionId == "finish_blueprint")
			{
				flag = false;
			}
			if (villagers.Count >= RequiredVillagerCount && (HasCardOnTop<BaseVillager>() || HasCardOnTop<Worker>()) && flag)
			{
				string actionId = GetActionId("CompleteHarvest");
				float num = 1f;
				List<CardData> list = villagers.FindAll((CardData x) => x is BaseVillager).ToList();
				if (list.Count > 0)
				{
					num = list.Max((CardData v) => ((BaseVillager)v).GetActionTimeModifier(actionId, this));
				}
				MyGameCard.StartTimer(num * HarvestTime, CompleteHarvest, StatusText, actionId);
			}
			else
			{
				MyGameCard.CancelTimer(GetActionId("CompleteHarvest"));
			}
		}
		base.UpdateCard();
	}

	public virtual void SendCard(GameCard card)
	{
		WorldManager.instance.StackSend(card, OutputDir);
	}

	public virtual ICardId GetCardToGive()
	{
		ICardId result = MyCardBag.GetCard();
		if (Id == "catacombs" && Amount == 1)
		{
			result = (CardId)"goblet";
		}
		if (Id == "cave" && Amount == 1)
		{
			result = (CardId)"treasure_map";
		}
		if (Id == "ruins" && Amount == 1)
		{
			result = (CardId)"blueprint_fountain_of_youth";
		}
		if (Id == "old_tome")
		{
			List<CardData> list = WorldManager.instance.CardDataPrefabs.Where((CardData x) => x.MyCardType == CardType.Ideas && !WorldManager.instance.HasFoundCard(x.Id) && !x.HideFromCardopedia).ToList();
			list.RemoveAll((CardData x) => x.CardUpdateType == CardUpdateType.Spirit);
			if (!WorldManager.instance.CurrentRunVariables.VisitedIsland)
			{
				list.RemoveAll((CardData x) => x.CardUpdateType == CardUpdateType.Island);
			}
			result = ((list.Count <= 0) ? ((CardId)"map") : ((CardId)list.Choose().Id));
		}
		return result;
	}

	[TimedAction("complete_harvest")]
	public void CompleteHarvest()
	{
		if (!IsUnlimited)
		{
			Amount--;
		}
		GameCard gameCard = null;
		if (HasCardOnTop<BaseVillager>() || HasCardOnTop<Worker>())
		{
			gameCard = MyGameCard.Child;
			gameCard.RotWobble(0.5f);
		}
		ICardId cardToGive = GetCardToGive();
		if (cardToGive != null && !string.IsNullOrEmpty(cardToGive.Id))
		{
			CardData cardData = WorldManager.instance.CreateCard(MyGameCard.transform.position, cardToGive, faceUp: false, checkAddToStack: false);
			WorldManager.instance.StackSendCheckTarget(MyGameCard, cardData.MyGameCard, OutputDir);
			if (cardData is Combatable combatable)
			{
				combatable.HealthPoints = combatable.ProcessedCombatStats.MaxHealth;
			}
			if (cardData is Creditcard creditcard)
			{
				creditcard.DollarCount = Random.Range(1, 3) * 10;
			}
		}
		if (!IsUnlimited && Amount <= 0)
		{
			if (gameCard != null && MyGameCard.HasParent && MyGameCard.Parent.CardData.Id == Id)
			{
				GameCard parent = MyGameCard.Parent;
				MyGameCard.RemoveFromStack();
				gameCard.SetParent(parent);
			}
			Emptied();
			MyGameCard.DestroyCard(spawnSmoke: true);
		}
		OnHarvestComplete();
		UpdateDescription();
	}

	public virtual void OnHarvestComplete()
	{
	}

	protected virtual void Emptied()
	{
	}
}
