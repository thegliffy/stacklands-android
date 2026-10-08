using System.Linq;
using UnityEngine;

public class Chest : CardData
{
	[ExtraData("coin_count")]
	[HideInInspector]
	public int CoinCount;

	public int MaxCoinCount = 100;

	public string HeldCardId = "gold";

	public string ChestTerm = "card_coin_chest_description_long";

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (otherCard.Id == Id)
		{
			return true;
		}
		if (otherCard.Id == HeldCardId)
		{
			return GetChestWithSpace() != null;
		}
		return false;
	}

	private Chest GetChestWithSpace()
	{
		GameCard gameCard = MyGameCard.GetAllCardsInStack().FirstOrDefault((GameCard x) => x.CardData is Chest chest && chest.CoinCount < chest.MaxCoinCount);
		if (gameCard == null)
		{
			return null;
		}
		return gameCard.CardData as Chest;
	}

	public override void UpdateCard()
	{
		Value = CoinCount;
		if (!MyGameCard.HasParent || MyGameCard.Parent.CardData is HeavyFoundation)
		{
			foreach (GameCard childCard in MyGameCard.GetChildCards())
			{
				if (!(childCard.CardData.Id != HeldCardId))
				{
					Chest chestWithSpace = GetChestWithSpace();
					if (!(chestWithSpace != null))
					{
						childCard.RemoveFromParent();
						break;
					}
					if (chestWithSpace.CoinCount < chestWithSpace.MaxCoinCount)
					{
						childCard.DestroyCard(spawnSmoke: true);
						chestWithSpace.CoinCount++;
					}
				}
			}
		}
		descriptionOverride = SokLoc.Translate(ChestTerm, LocParam.Create("count", CoinCount.ToString()), LocParam.Create("max_count", MaxCoinCount.ToString()), LocParam.Create("goldicon", Icons.Gold), LocParam.Create("shellicon", Icons.Shell));
		base.UpdateCard();
	}

	public override void Clicked()
	{
		int a = 5;
		if (CoinCount > 0)
		{
			int num = Mathf.Min(a, CoinCount);
			GameCard gameCard = WorldManager.instance.CreateCardStack(base.transform.position + Vector3.up * 0.2f, num, HeldCardId, checkAddToStack: false);
			WorldManager.instance.StackSend(gameCard.GetRootCard(), OutputDir, null, sendToChest: false);
			CoinCount -= num;
		}
		base.Clicked();
	}
}
