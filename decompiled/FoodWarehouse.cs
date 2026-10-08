using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FoodWarehouse : Food
{
	private int MaxFoodValue = 999;

	[ExtraData("resource_id")]
	[HideInInspector]
	public string HeldCardId = "";

	private CardConnector outputConnector;

	public override bool DetermineCanHaveCardsWhenIsRoot => true;

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (otherCard is Hotpot)
		{
			return false;
		}
		if (otherCard is Food { FoodValue: >0 })
		{
			if (string.IsNullOrEmpty(HeldCardId))
			{
				return true;
			}
			if (!string.IsNullOrEmpty(HeldCardId) && otherCard.Id == HeldCardId)
			{
				return true;
			}
			return false;
		}
		return false;
	}

	public override bool CanHaveCardsWhileHasStatus()
	{
		return true;
	}

	public override void UpdateCard()
	{
		MyGameCard.SpecialValue = FoodValue;
		MyGameCard.SpecialIcon.sprite = SpriteManager.instance.FoodIcon;
		if ((!MyGameCard.HasParent || MyGameCard.Parent.CardData is HeavyFoundation) && MyGameCard.HasChild && !(MyGameCard.Child.CardData is FoodWarehouse) && (string.IsNullOrEmpty(HeldCardId) || (!string.IsNullOrEmpty(HeldCardId) && MyGameCard.Child.CardData.Id == HeldCardId)))
		{
			StoreFood();
		}
		if (!string.IsNullOrEmpty(HeldCardId))
		{
			Food food = WorldManager.instance.GameDataLoader.GetCardFromId(HeldCardId) as Food;
			nameOverride = SokLoc.Translate("card_food_warehouse_name_long", LocParam.Create("food", WorldManager.instance.GameDataLoader.GetCardFromId(HeldCardId).Name));
			descriptionOverride = SokLoc.Translate("card_food_warehouse_description_long", LocParam.Create("food", WorldManager.instance.GameDataLoader.GetCardFromId(HeldCardId).Name), LocParam.Create("amount", (FoodValue / food.FoodValue).ToString()));
		}
		else
		{
			nameOverride = SokLoc.Translate("card_food_warehouse_name");
			descriptionOverride = null;
		}
		if (outputConnector == null)
		{
			outputConnector = GetOutputConnector();
		}
		if (FoodValue > 0 && outputConnector?.ConnectedNode != null)
		{
			MyGameCard.StartTimer(10f, OutputCard, SokLoc.Translate("idea_resourcechest_status_2"), GetActionId("OutputCard"));
		}
		else
		{
			MyGameCard.CancelTimer(GetActionId("OutputCard"));
		}
		base.UpdateCard();
		if (string.IsNullOrEmpty(HeldCardId))
		{
			Icon = SpriteManager.instance.EmptyTexture;
		}
		else
		{
			Icon = WorldManager.instance.GetCardPrefab(HeldCardId).Icon;
		}
		MyGameCard.UpdateIcon();
	}

	public CardConnector GetOutputConnector()
	{
		CardConnector result = null;
		for (int i = 0; i < MyGameCard.CardConnectorChildren.Count; i++)
		{
			CardConnector cardConnector = MyGameCard.CardConnectorChildren[i];
			if (cardConnector != null && cardConnector.ConnectionType == ConnectionType.Transport && cardConnector.CardDirection == CardDirection.output)
			{
				result = cardConnector;
			}
		}
		return result;
	}

	public void StoreFood()
	{
		foreach (GameCard childCard in MyGameCard.GetChildCards())
		{
			if (string.IsNullOrEmpty(HeldCardId))
			{
				HeldCardId = childCard.CardData.Id;
			}
			if (childCard.CardData.Id != HeldCardId || childCard.CardData is Hotpot || childCard.CardData is FoodWarehouse)
			{
				continue;
			}
			if (childCard.SpecialValue.HasValue && FoodValue + childCard.SpecialValue <= MaxFoodValue)
			{
				FoodValue += childCard.SpecialValue.Value;
				childCard.DestroyCard(spawnSmoke: true);
			}
			else if (childCard.CardData is Food food)
			{
				int num = Mathf.Min(MaxFoodValue - FoodValue, food.FoodValue);
				FoodValue += num;
				food.FoodValue -= num;
				if (food.FoodValue <= 0)
				{
					childCard.DestroyCard(spawnSmoke: true);
				}
				else
				{
					childCard.RemoveFromParent();
				}
			}
		}
	}

	public GameCard RemoveFood(int count, bool checkOutput = false)
	{
		Food food = WorldManager.instance.GameDataLoader.GetCardFromId(HeldCardId) as Food;
		List<GameCard> list = new List<GameCard>();
		for (int i = 0; i < count; i++)
		{
			CardData cardData;
			if (FoodValue >= food.FoodValue)
			{
				cardData = WorldManager.instance.CreateCard(base.transform.position, HeldCardId, faceUp: true, checkAddToStack: false);
				FoodValue -= food.FoodValue;
			}
			else
			{
				int num = Mathf.Min(FoodValue, food.FoodValue);
				cardData = WorldManager.instance.CreateCard(base.transform.position, HeldCardId, faceUp: true, checkAddToStack: false);
				if (cardData is Food food2)
				{
					food2.FoodValue = num;
				}
				FoodValue -= num;
			}
			if (cardData != null)
			{
				list.Add(cardData.MyGameCard);
			}
			if (FoodValue <= 0)
			{
				FoodValue = 0;
				break;
			}
		}
		WorldManager.instance.Restack(list);
		if (checkOutput)
		{
			WorldManager.instance.StackSendCheckTarget(MyGameCard, list[0], OutputDir);
		}
		else
		{
			WorldManager.instance.StackSend(list[0], OutputDir);
		}
		return list[0].GetRootCard();
	}

	[TimedAction("output_card")]
	public void OutputCard()
	{
		if (FoodValue > 0)
		{
			RemoveFood(1, checkOutput: true);
		}
	}

	public override void Clicked()
	{
		int count = 1;
		if (InputController.instance.GetKey(Key.LeftShift) || InputController.instance.GetKey(Key.RightShift))
		{
			count = 5;
		}
		if (FoodValue > 0)
		{
			RemoveFood(count);
		}
		if (FoodValue == 0)
		{
			HeldCardId = null;
		}
		base.Clicked();
	}
}
