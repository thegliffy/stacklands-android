using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Worker : CardData, HousingConsumer
{
	public int HousingSpaceRequired = 1;

	public WorkerType WorkerType;

	[HideInInspector]
	[ExtraData("housingUniqueId")]
	public string HousingUniqueId;

	[HideInInspector]
	public Apartment Housing
	{
		get
		{
			if (HousingUniqueId != null)
			{
				GameCard cardWithUniqueId = WorldManager.instance.GetCardWithUniqueId(HousingUniqueId);
				if (cardWithUniqueId != null)
				{
					return cardWithUniqueId.CardData as Apartment;
				}
			}
			return null;
		}
		set
		{
			HousingUniqueId = ((value != null) ? value.UniqueId : "");
		}
	}

	public string HousingId => HousingUniqueId;

	public override void OnInitialCreate()
	{
		Housing = null;
		QuestManager.instance.SpecialActionComplete("worker_created", this);
		base.OnInitialCreate();
	}

	public override void UpdateCard()
	{
		GameBoard currentBoard = WorldManager.instance.CurrentBoard;
		if ((object)currentBoard == null || currentBoard.Location != Location.Cities)
		{
			RemoveStatusEffect<StatusEffect_Homeless>();
			base.UpdateCard();
			return;
		}
		Apartment housing = Housing;
		bool flag = housing != null && !housing.IsDamaged && housing.HasEnergyInput();
		if (GetHousingSpaceRequired() == 0)
		{
			flag = true;
		}
		if (!flag && !HasStatusEffectOfType<StatusEffect_Homeless>())
		{
			AddStatusEffect(new StatusEffect_Homeless());
		}
		if (flag && HasStatusEffectOfType<StatusEffect_Homeless>())
		{
			RemoveStatusEffect<StatusEffect_Homeless>();
		}
		if (CitiesManager.instance.WorkersOnBoard.Count <= 1)
		{
			CitiesValue = -1;
		}
		else if (WorkerType == WorkerType.Educated)
		{
			CitiesValue = 30;
		}
		else if (WorkerType == WorkerType.Normal)
		{
			CitiesValue = 20;
		}
		else if (WorkerType == WorkerType.Robot)
		{
			CitiesValue = 40;
		}
		base.UpdateCard();
	}

	public override void UpdateCardText()
	{
		if (!string.IsNullOrEmpty(CustomName))
		{
			nameOverride = CustomName;
		}
	}

	public float GetActionTimeModifier()
	{
		if (Id == "educated_worker")
		{
			return 0.75f;
		}
		if (Id == "genius" || Id == "robot_genius")
		{
			return 0.5f;
		}
		return 1f;
	}

	protected override bool CanHaveCard(CardData otherCard)
	{
		return otherCard is Worker;
	}

	public override void StoppedDragging()
	{
		List<CardData> list = CardsInStackMatchingPredicate((CardData x) => x is Worker);
		List<GameCard> list2 = (from x in MyGameCard.GetAllCardsInStack()
			where x.CardData.WorkerAmount > 0
			select x).ToList();
		if (list2.Count > 0)
		{
			foreach (GameCard item in list2)
			{
				foreach (Worker item2 in list)
				{
					if (item2 != null)
					{
						item2.TryEquipOnCard(item);
					}
				}
			}
		}
		else
		{
			foreach (Worker item3 in list)
			{
				item3.TryUnequipSelf();
			}
		}
		base.StoppedDragging();
	}

	private void TryUnequipSelf()
	{
		MyGameCard.WorkerHolder?.UnequipWorker(MyGameCard);
	}

	private void TryEquipOnCard(GameCard card)
	{
		if ((object)card == null || !(card.CardData?.WorkerAmount <= 0))
		{
			if (MyGameCard.WorkerHolder != null)
			{
				MyGameCard.WorkerHolder.UnequipWorker(MyGameCard);
			}
			if (card != null && card.CardData.WorkerAmount > 0)
			{
				card.OpenInventory(showInventory: true);
				card.CardData.EquipWorker(this);
			}
		}
	}

	public GameCard GetGameCard()
	{
		return MyGameCard;
	}

	public int GetHousingSpaceRequired()
	{
		return HousingSpaceRequired;
	}

	public WorkerType GetWorkerType()
	{
		return WorkerType;
	}

	public override void OnSellCard()
	{
		if (Housing != null)
		{
			Housing.UsedSpace -= GetHousingSpaceRequired();
			Housing = null;
		}
		QuestManager.instance.SpecialActionComplete("worker_removed", this);
		base.OnSellCard();
	}

	public override void OnDestroyCard()
	{
		if (Housing != null)
		{
			Housing.UsedSpace -= GetHousingSpaceRequired();
			Housing = null;
		}
		QuestManager.instance.SpecialActionComplete("worker_removed", this);
		base.OnDestroyCard();
	}
}
