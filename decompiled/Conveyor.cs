using UnityEngine;

public class Conveyor : CardData
{
	public float ExtraSideDistance = 0.01f;

	[ExtraData("direction")]
	[HideInInspector]
	public int Direction;

	public float TotalTime = 5f;

	private Vector2[] corners = new Vector2[4];

	private Vector3 directionVector
	{
		get
		{
			if (Direction == 0)
			{
				return Vector3.back;
			}
			if (Direction == 1)
			{
				return Vector3.left;
			}
			if (Direction == 2)
			{
				return Vector3.forward;
			}
			if (Direction == 3)
			{
				return Vector3.right;
			}
			return Vector3.back;
		}
	}

	protected override bool CanToggleOnOff()
	{
		if (WorldManager.instance.CurrentBoard.Id == "cities")
		{
			return true;
		}
		return false;
	}

	protected override bool CanHaveCard(CardData otherCard)
	{
		return false;
	}

	private bool CanBeInputCard(CardData card)
	{
		if (card.MyGameCard.Velocity.HasValue || card.MyGameCard.BounceTarget != null)
		{
			return false;
		}
		if (MyGameCard.IsParentOf(card.MyGameCard))
		{
			return false;
		}
		if (card is ResourceChest resourceChest)
		{
			if (string.IsNullOrEmpty(resourceChest.HeldCardId))
			{
				return false;
			}
			return CanBeConveyed(resourceChest.HeldCardId);
		}
		if (card is ResourceMagnet resourceMagnet)
		{
			if (string.IsNullOrEmpty(resourceMagnet.PullCardId))
			{
				return false;
			}
			return CanBeConveyed(resourceMagnet.PullCardId);
		}
		if (CanBeConveyed(card))
		{
			if (card.MyGameCard.HasChild)
			{
				return false;
			}
			return true;
		}
		return false;
	}

	private bool CanBeConveyed(string cardId)
	{
		CardData cardPrefab = WorldManager.instance.GetCardPrefab(cardId);
		return CanBeConveyed(cardPrefab);
	}

	private CardData GetConveyableCardFromInputCard(CardData card)
	{
		if (card is ResourceChest { ResourceCount: >0 } resourceChest)
		{
			return resourceChest.RemoveResources(1).CardData;
		}
		if (card is ResourceMagnet resourceMagnet && resourceMagnet.MyGameCard.HasChild)
		{
			return resourceMagnet.MyGameCard.GetLeafCard().CardData;
		}
		if (CanBeConveyed(card))
		{
			return card;
		}
		return null;
	}

	private bool InputCardHasConveyableCard(CardData card)
	{
		if (card is ResourceChest resourceChest)
		{
			return resourceChest.ResourceCount > 0;
		}
		if (card is ResourceMagnet resourceMagnet && resourceMagnet.MyGameCard.HasChild)
		{
			return true;
		}
		if (CanBeConveyed(card))
		{
			return true;
		}
		return false;
	}

	private CardData GetPrefabForId(string id)
	{
		return WorldManager.instance.GetCardPrefab(id);
	}

	private CardData GetInputCardConveyablePrefab(CardData card)
	{
		if (card is ResourceChest resourceChest)
		{
			return GetPrefabForId(resourceChest.HeldCardId);
		}
		if (card is ResourceMagnet resourceMagnet)
		{
			return GetPrefabForId(resourceMagnet.PullCardId);
		}
		if (CanBeConveyed(card))
		{
			return GetPrefabForId(card.Id);
		}
		return null;
	}

	private CardData GetInputCard(bool allowDraggingCards)
	{
		return WorldManager.instance.GetBestCardInDirection(MyGameCard, directionVector, allowDraggingCards, (GameCard card) => CanBeInputCard(card.CardData))?.CardData;
	}

	private bool CanBeConveyed(CardData otherCard)
	{
		if (otherCard.MyCardType != CardType.Resources && otherCard.MyCardType != CardType.Food && otherCard.MyCardType != CardType.Humans)
		{
			if (otherCard is Mob mob)
			{
				return !mob.IsAggressive;
			}
			return false;
		}
		return true;
	}

	public override void UpdateCard()
	{
		if (MyGameCard.IsDemoCard)
		{
			return;
		}
		bool flag = true;
		if (MyGameCard.Velocity.HasValue)
		{
			flag = false;
		}
		CardData cardData = null;
		if (flag)
		{
			cardData = GetInputCard(allowDraggingCards: true);
		}
		if (cardData != null && InputCardHasConveyableCard(cardData))
		{
			CardData inputCardConveyablePrefab = GetInputCardConveyablePrefab(cardData);
			string status = SokLoc.Translate("card_conveyor_status", LocParam.Create("resource", inputCardConveyablePrefab.Name));
			MyGameCard.StartTimer(TotalTime, LoadCard, status, GetActionId("LoadCard"));
		}
		else
		{
			MyGameCard.CancelAnyTimer();
		}
		CardData outputCard = null;
		if (cardData != null)
		{
			CardData inputCardConveyablePrefab2 = GetInputCardConveyablePrefab(cardData);
			if (inputCardConveyablePrefab2 != null)
			{
				outputCard = WorldManager.instance.GetTargetCard(MyGameCard, inputCardConveyablePrefab2, -directionVector, allowDraggedCards: true, cardData.MyGameCard)?.CardData;
			}
		}
		DrawArrows(cardData, outputCard);
		base.UpdateCard();
	}

	public override void Clicked()
	{
		Direction = (Direction + 1) % 4;
		base.Clicked();
	}

	[TimedAction("load_card")]
	public void LoadCard()
	{
		CardData inputCard = GetInputCard(allowDraggingCards: false);
		if (inputCard == null)
		{
			return;
		}
		CardData conveyableCardFromInputCard = GetConveyableCardFromInputCard(inputCard);
		if (conveyableCardFromInputCard == null)
		{
			return;
		}
		conveyableCardFromInputCard.MyGameCard.RemoveFromStack();
		GameCard targetCard = WorldManager.instance.GetTargetCard(MyGameCard, conveyableCardFromInputCard, -directionVector, allowDraggedCards: false, inputCard.MyGameCard);
		if (targetCard != null)
		{
			SendToTargetCard(conveyableCardFromInputCard.MyGameCard, targetCard);
		}
		else
		{
			if (conveyableCardFromInputCard.MyGameCard.BounceTarget == inputCard.MyGameCard)
			{
				conveyableCardFromInputCard.MyGameCard.BounceTarget = null;
			}
			conveyableCardFromInputCard.MyGameCard.SendToPosition(MyGameCard.transform.position - directionVector);
		}
		QuestManager.instance.SpecialActionComplete("use_conveyor");
	}

	private void SendToTargetCard(GameCard card, GameCard targetCard)
	{
		Vector3 vector = targetCard.transform.position - card.transform.position;
		vector.y = 0f;
		Vector3 value = new Vector3(vector.x * 4f, 7f, vector.z * 4f);
		card.BounceTarget = targetCard.GetRootCard();
		card.Velocity = value;
	}

	private Vector2 GetPointOnCardEdge(Vector2 start, Vector2 end, GameCard card)
	{
		Bounds bounds = card.GetBounds();
		corners[0] = new Vector2(bounds.min.x, bounds.min.z);
		corners[1] = new Vector2(bounds.max.x, bounds.min.z);
		corners[2] = new Vector2(bounds.max.x, bounds.max.z);
		corners[3] = new Vector2(bounds.min.x, bounds.max.z);
		for (int i = 0; i < 4; i++)
		{
			Vector2 p = corners[i];
			Vector2 p2 = corners[(i + 1) % 4];
			if (MathHelper.LineSegmentsIntersection(start, end, p, p2, out var intersection, out var _))
			{
				return intersection;
			}
		}
		return start;
	}

	private Vector3 TransformToEdge(Vector3 start, Vector3 end, GameCard card, float dir)
	{
		Vector2 start2 = new Vector2(start.x, start.z);
		Vector2 end2 = new Vector2(end.x, end.z);
		Vector2 pointOnCardEdge = GetPointOnCardEdge(start2, end2, card);
		return new Vector3(pointOnCardEdge.x, 0f, pointOnCardEdge.y) + (start - end).normalized * ExtraSideDistance * dir;
	}

	private void DrawInputArrow(CardData inputCard)
	{
		Vector3 position = MyGameCard.transform.position;
		Vector3 start = ((!(inputCard != null)) ? (MyGameCard.transform.position + directionVector * 0.5f) : TransformToEdge(inputCard.transform.position, position, inputCard.MyGameCard, -1f));
		position = TransformToEdge(start, position, MyGameCard, 1f);
		DrawManager.instance.DrawShape(new ConveyorArrow
		{
			Start = start,
			End = position
		});
	}

	private void DrawOutputArrow(CardData outputCard)
	{
		Vector3 position = MyGameCard.transform.position;
		Vector3 end = ((!(outputCard != null)) ? (MyGameCard.transform.position - directionVector * 0.5f) : TransformToEdge(position, outputCard.transform.position, outputCard.MyGameCard, 1f));
		position = TransformToEdge(position, end, MyGameCard, -1f);
		DrawManager.instance.DrawShape(new ConveyorArrow
		{
			Start = position,
			End = end
		});
	}

	private void DrawArrows(CardData inputCard, CardData outputCard)
	{
		DrawInputArrow(inputCard);
		DrawOutputArrow(outputCard);
	}
}
