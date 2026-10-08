using System.Collections.Generic;
using UnityEngine;

public class WishingWell : CardData
{
	public int WishCost = 500;

	public List<AudioClip> WishSound;

	public Sprite SpecialIcon;

	[ExtraData("coin_count")]
	[HideInInspector]
	public int CoinCount;

	[ExtraData("wish_count")]
	[HideInInspector]
	public int WishCount;

	private string HeldCardId = "gold";

	protected override bool CanHaveCard(CardData otherCard)
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

	public override void UpdateCardText()
	{
		if (WishCount > 0)
		{
			descriptionOverride = SokLoc.Translate("card_wishing_well_description_long", LocParam.Plural("amount", WishCount), LocParam.Create("count", WishCost.ToString()));
		}
		else
		{
			descriptionOverride = SokLoc.Translate("card_wishing_well_description", LocParam.Create("count", WishCost.ToString()));
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
					if (chest.CoinCount < WishCost - CoinCount)
					{
						CoinCount += chest.CoinCount;
						chest.CoinCount = 0;
						WorldManager.instance.CreateSmoke(MyGameCard.transform.position);
						chest.MyGameCard.RemoveFromStack();
						chest.MyGameCard.SendIt();
					}
					else if (chest.CoinCount >= WishCost - CoinCount)
					{
						chest.CoinCount -= WishCost - CoinCount;
						CoinCount = WishCost;
						WorldManager.instance.CreateSmoke(MyGameCard.transform.position);
						chest.MyGameCard.RemoveFromStack();
						chest.MyGameCard.SendIt();
					}
				}
				if (!(childCard.CardData.Id != HeldCardId))
				{
					if (CoinCount >= WishCost)
					{
						childCard.RemoveFromParent();
						break;
					}
					childCard.DestroyCard(spawnSmoke: true);
					CoinCount++;
				}
			}
			if (CoinCount == WishCost)
			{
				GiveWish();
			}
		}
		base.UpdateCard();
	}

	private void GiveWish()
	{
		AudioManager.me.PlaySound2D(WishSound, 1f, 0.1f);
		WorldManager.instance.CreateSmoke(base.transform.position);
		CoinCount = 0;
		WishCount++;
		switch (WishCount)
		{
		case 1:
			WorldManager.instance.QueueCutscene(Cutscenes.Wish1(this));
			break;
		case 2:
			WorldManager.instance.QueueCutscene(Cutscenes.Wish2(this));
			break;
		case 5:
			WorldManager.instance.QueueCutscene(Cutscenes.Wish5(this));
			break;
		case 10:
			WorldManager.instance.QueueCutscene(Cutscenes.Wish10(this));
			break;
		case 20:
			WorldManager.instance.QueueCutscene(Cutscenes.Wish20(this));
			break;
		case 50:
			WorldManager.instance.QueueCutscene(Cutscenes.Wish50(this));
			break;
		}
	}
}
