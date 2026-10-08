using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Mob : Combatable
{
	private delegate bool DropFilter(string cardId);

	[HideInInspector]
	public float MoveTimer;

	[Header("Mob")]
	public float MoveTime = 3f;

	public bool IsAggressive;

	public bool AlwaysDrop;

	[HideInInspector]
	public Combatable CurrentTarget;

	public CardBag Drops;

	protected bool moveFlag;

	private List<Combatable> overlappingCombatables = new List<Combatable>();

	public virtual bool CanMove => MyGameCard.GetCardWithStatusInStack() == null;

	public override bool CanBeDragged => false;

	public override bool CanBePushedBy(CardData otherCard)
	{
		return otherCard is Mob;
	}

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (otherCard.Id == "bone" && Id == "wolf")
		{
			return true;
		}
		if (otherCard.Id == "milk" && Id == "feral_cat")
		{
			return true;
		}
		return AllCardsInStackMatchPred(otherCard, (CardData x) => x is Combatable { CanAttack: not false } combatable && !(combatable is Animal));
	}

	public override void UpdateCard()
	{
		moveFlag = false;
		bool flag = CanMove && !base.InConflict && MoveTime != -1f;
		if (MyGameCard.BounceTarget != null)
		{
			flag = false;
		}
		if (flag)
		{
			float num = ((this is Enemy) ? 2f : 1f);
			if (WorldManager.instance.CurrentBoard != null && WorldManager.instance.CurrentBoard.Id == "forest")
			{
				num = 3f;
			}
			MoveTimer += Time.deltaTime * WorldManager.instance.TimeScale * num;
			if (MoveTimer >= MoveTime)
			{
				moveFlag = true;
				MoveTimer = 0f;
				Move();
			}
		}
		else
		{
			MoveTimer = 0f;
		}
		if (IsAggressive && !base.InConflict && WorldManager.instance.TimeScale > 0f)
		{
			if (CurrentTarget == null || CurrentTarget.MyGameCard.MyBoard != MyGameCard.MyBoard)
			{
				CurrentTarget = FindTarget();
			}
			foreach (Combatable overlappingCombatable in GetOverlappingCombatables())
			{
				if (overlappingCombatable.InConflict)
				{
					overlappingCombatable.MyConflict.JoinConflict(this);
					break;
				}
				if (overlappingCombatable.Team != base.Team)
				{
					List<GameCard> list = (from x in overlappingCombatable.MyGameCard.GetAllCardsInStack()
						where x.Combatable != null
						select x).ToList();
					list.Add(MyGameCard);
					list = list.Distinct().ToList();
					WorldManager.instance.Restack(list);
					break;
				}
			}
		}
		base.UpdateCard();
	}

	public override void Die()
	{
		TryDropItems();
		base.Die();
	}

	public void TryDropItems()
	{
		List<GameCard> list = new List<GameCard>();
		bool flag = false;
		if (WorldManager.instance.CurrentRunVariables.CanDropItem)
		{
			flag = TryDropEquipment();
			if (flag)
			{
				Debug.Log("Dropped special equipment!");
				WorldManager.instance.CurrentRunVariables.CanDropItem = false;
			}
		}
		if (flag)
		{
			return;
		}
		Debug.Log("Dropped normal item!");
		if (Drops.CardBagType == CardBagType.Chances && Drops.Chances.Count == 0)
		{
			base.Die();
			return;
		}
		for (int i = 0; i < Drops.CardsInPack; i++)
		{
			ICardId cardFiltered = Drops.GetCardFiltered(Filter, removeCard: false);
			if (cardFiltered != null && !string.IsNullOrWhiteSpace(cardFiltered.Id))
			{
				CardData cardFromId = WorldManager.instance.GameDataLoader.GetCardFromId(cardFiltered.Id);
				if (cardFromId is Equipable)
				{
					WorldManager.instance.CurrentRunVariables.CanDropItem = false;
				}
				CardData cardData = WorldManager.instance.CreateCard(base.transform.position, cardFromId, faceUp: true, checkAddToStack: false);
				cardData.MyGameCard.SendIt();
				list.Add(cardData.MyGameCard);
			}
		}
		if (list.Count <= 0 || !WorldManager.instance.StackAllSame(list[0]))
		{
			return;
		}
		foreach (GameCard item in list)
		{
			item.Velocity = null;
		}
		WorldManager.instance.Restack(list);
		WorldManager.instance.StackSend(list[0], OutputDir);
	}

	private bool Filter(string cardId)
	{
		if (AlwaysDrop)
		{
			return true;
		}
		if (WorldManager.instance.GameDataLoader.GetCardFromId(cardId) is Equipable && !WorldManager.instance.CurrentRunVariables.CanDropItem)
		{
			return false;
		}
		if (WorldManager.instance.CurrentBoard != null && WorldManager.instance.CurrentBoard.Id == "forest")
		{
			return ForestCombatManager.instance.CanDropCard(cardId);
		}
		return true;
	}

	public bool TryDropEquipment()
	{
		if (!HasInventory)
		{
			return false;
		}
		List<Equipable> list = (from x in GetAllEquipables()
			orderby Random.value
			select x).ToList();
		if (list.Count == 0)
		{
			return false;
		}
		List<Equipable> list2 = new List<Equipable>();
		foreach (Equipable item in list)
		{
			list2.Add(item);
		}
		list2.RemoveAll((Equipable x) => x.blueprint != null && WorldManager.instance.HasFoundCard(x.Id));
		if (list2.Count == 0)
		{
			return false;
		}
		CardData cardData = WorldManager.instance.CreateCard(base.transform.position, list2[0], faceUp: true, checkAddToStack: false);
		if (cardData is Equipable equipable && equipable.blueprint != null)
		{
			WorldManager.instance.CreateCard(base.transform.position, equipable.blueprint, faceUp: true, checkAddToStack: false).MyGameCard.SetChild(cardData.MyGameCard);
		}
		cardData.MyGameCard.SendIt();
		return true;
	}

	public List<Combatable> GetOverlappingCombatables()
	{
		overlappingCombatables.Clear();
		foreach (GameCard overlappingCard in MyGameCard.GetOverlappingCards())
		{
			foreach (GameCard item in overlappingCard.GetAllCardsInStack())
			{
				if (item.Combatable != null && !item.BeingDragged)
				{
					overlappingCombatables.Add(item.Combatable);
				}
			}
		}
		return overlappingCombatables;
	}

	protected virtual void Move()
	{
		MyGameCard.SendIt();
	}

	protected virtual Combatable FindTarget()
	{
		if (WorldManager.instance.CurrentBoard.Id == "cities")
		{
			return WorldManager.instance.GetCard<CitiesCombatable>();
		}
		return WorldManager.instance.GetCard<BaseVillager>();
	}
}
