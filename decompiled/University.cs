using System.Collections.Generic;
using UnityEngine;

public class University : CardData
{
	public int InventionCost = 50;

	[Card]
	public List<string> BlueprintDrops = new List<string>();

	public List<AudioClip> InventionSound;

	public Sprite SpecialIcon;

	[ExtraData("coin_count")]
	[HideInInspector]
	public int CoinCount;

	private string HeldCardId = "gold";

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (!AllInventionsFound())
		{
			if (!(otherCard.Id == HeldCardId))
			{
				if (otherCard is Chest chest)
				{
					return chest.HeldCardId == HeldCardId;
				}
				return false;
			}
			return true;
		}
		return false;
	}

	public override void UpdateCardText()
	{
		if (AllInventionsFound())
		{
			descriptionOverride = SokLoc.Translate("card_university_description_completed");
		}
		else if (CoinCount > 0)
		{
			descriptionOverride = SokLoc.Translate("card_university_description_long", LocParam.Create("count", CoinCount.ToString()), LocParam.Create("max_count", InventionCost.ToString()));
		}
		else
		{
			descriptionOverride = SokLoc.Translate("card_university_description", LocParam.Create("max_count", InventionCost.ToString()));
		}
	}

	public override void UpdateCard()
	{
		if (!MyGameCard.HasParent || MyGameCard.Parent.CardData is HeavyFoundation)
		{
			foreach (GameCard childCard in MyGameCard.GetChildCards())
			{
				if (childCard.CardData is Chest chest)
				{
					if (chest.CoinCount < InventionCost - CoinCount)
					{
						CoinCount += chest.CoinCount;
						chest.CoinCount = 0;
						WorldManager.instance.CreateSmoke(MyGameCard.transform.position);
						chest.MyGameCard.RemoveFromStack();
						chest.MyGameCard.SendIt();
					}
					else if (chest.CoinCount >= InventionCost - CoinCount)
					{
						chest.CoinCount -= InventionCost - CoinCount;
						CoinCount = InventionCost;
						WorldManager.instance.CreateSmoke(MyGameCard.transform.position);
						chest.MyGameCard.RemoveFromStack();
						chest.MyGameCard.SendIt();
					}
				}
				if (!(childCard.CardData.Id != HeldCardId))
				{
					if (CoinCount >= InventionCost)
					{
						childCard.RemoveFromParent();
						break;
					}
					childCard.DestroyCard(spawnSmoke: true);
					CoinCount++;
				}
			}
			if (CoinCount == InventionCost)
			{
				MyGameCard.StartTimer(10f, GiveInvention, SokLoc.Translate("card_university_status"), GetActionId("GiveInvention"));
			}
		}
		if (AllInventionsFound())
		{
			MyGameCard.CancelTimer(GetActionId("GiveInvention"));
		}
		base.UpdateCard();
	}

	private bool AllInventionsFound()
	{
		bool result = true;
		foreach (string blueprintDrop in BlueprintDrops)
		{
			if (!WorldManager.instance.HasFoundCard(blueprintDrop))
			{
				result = false;
				break;
			}
		}
		if (WorldManager.instance.IsCitiesDlcActive() && !WorldManager.instance.HasFoundCard("industrial_revolution"))
		{
			return false;
		}
		return result;
	}

	[TimedAction("give_invention")]
	public void GiveInvention()
	{
		if (WorldManager.instance.IsCitiesDlcActive() && !WorldManager.instance.HasFoundCard("industrial_revolution"))
		{
			CardData cardData = WorldManager.instance.CreateCard(MyGameCard.transform.position, "industrial_revolution", faceUp: true, checkAddToStack: false);
			WorldManager.instance.CreateSmoke(cardData.transform.position);
			cardData.MyGameCard.SendIt();
			AudioManager.me.PlaySound2D(InventionSound, 1f, 0.1f);
			CoinCount = 0;
			return;
		}
		foreach (string blueprintDrop in BlueprintDrops)
		{
			Blueprint blueprint = WorldManager.instance.GameDataLoader.GetCardFromId(blueprintDrop) as Blueprint;
			if ((bool)blueprint && !WorldManager.instance.HasFoundCard(blueprint.Id))
			{
				CardData cardData2 = WorldManager.instance.CreateCard(MyGameCard.transform.position, blueprint, faceUp: true, checkAddToStack: false);
				WorldManager.instance.CreateSmoke(cardData2.transform.position);
				cardData2.MyGameCard.SendIt();
				AudioManager.me.PlaySound2D(InventionSound, 1f, 0.1f);
				CoinCount = 0;
				break;
			}
		}
	}
}
