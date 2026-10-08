using UnityEngine;

public class Merchant : CardData
{
	public int AmountNeeded = 100;

	[ExtraData("amountGiven")]
	public int AmountGiven;

	private string HeldCardId = "gold";

	public AudioClip BuySound;

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

	public override void UpdateCard()
	{
		if (!MyGameCard.HasParent || MyGameCard.Parent.CardData is HeavyFoundation)
		{
			foreach (GameCard childCard in MyGameCard.GetChildCards())
			{
				if (childCard.CardData is Chest chest)
				{
					if (chest.CoinCount < AmountNeeded - AmountGiven)
					{
						AmountGiven += chest.CoinCount;
						chest.CoinCount = 0;
						WorldManager.instance.CreateSmoke(MyGameCard.transform.position);
						chest.MyGameCard.RemoveFromStack();
						chest.MyGameCard.SendIt();
					}
					else if (chest.CoinCount >= AmountNeeded - AmountGiven)
					{
						chest.CoinCount -= AmountNeeded - AmountGiven;
						AmountGiven = AmountNeeded;
						WorldManager.instance.CreateSmoke(MyGameCard.transform.position);
						chest.MyGameCard.RemoveFromStack();
						chest.MyGameCard.SendIt();
					}
				}
				if (!(childCard.CardData.Id != HeldCardId))
				{
					if (AmountGiven >= AmountNeeded)
					{
						childCard.RemoveFromParent();
						break;
					}
					childCard.DestroyCard(spawnSmoke: true);
					AmountGiven++;
				}
			}
			if (AmountGiven == AmountNeeded)
			{
				WorldManager.instance.CreateCard(base.Position, "dragon_egg").MyGameCard.SendIt();
				WorldManager.instance.CreateSmoke(base.Position);
				AudioManager.me.PlaySound2D(BuySound, 1f, 0.3f);
				MyGameCard.DestroyCard();
			}
		}
		base.UpdateCard();
	}

	public override void UpdateCardText()
	{
		if (AmountGiven > 0)
		{
			descriptionOverride = SokLoc.Translate("card_merchant_description_2", LocParam.Create("coinsNeeded", (AmountNeeded - AmountGiven).ToString()));
		}
		else
		{
			descriptionOverride = "";
		}
		base.UpdateCardText();
	}
}
