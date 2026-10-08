using UnityEngine;

public class FilteredJunction : CardData
{
	[ExtraData("filtered_card")]
	[HideInInspector]
	public string FilteredCard;

	[Term]
	public string NameOverride;

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (otherCard.MyCardType == CardType.Structures)
		{
			return false;
		}
		if (otherCard.MyCardType == CardType.Humans)
		{
			return false;
		}
		return true;
	}

	public override void UpdateCard()
	{
		if (MyGameCard.HasChild)
		{
			if (string.IsNullOrEmpty(FilteredCard))
			{
				FilteredCard = MyGameCard.Child.CardData.Id;
				MyGameCard.Child.DestroyCard(spawnSmoke: true);
				return;
			}
			for (int num = MyGameCard.GetChildCards().Count - 1; num >= 0; num--)
			{
				int outputIndex = -1;
				GameCard gameCard = MyGameCard.GetChildCards()[num];
				if (!string.IsNullOrEmpty(FilteredCard))
				{
					outputIndex = ((gameCard.CardData.Id == FilteredCard) ? 1 : 0);
				}
				gameCard.RemoveFromStack();
				WorldManager.instance.StackSendCheckTarget(MyGameCard, gameCard, OutputDir, null, sendToChest: true, outputIndex);
			}
		}
		base.UpdateCard();
	}

	public override void Clicked()
	{
		if (!string.IsNullOrEmpty(FilteredCard))
		{
			WorldManager.instance.CreateCard(base.Position, FilteredCard, faceUp: true, checkAddToStack: false).MyGameCard.SendIt();
			FilteredCard = "";
		}
		base.Clicked();
	}

	public override void UpdateCardText()
	{
		if (!string.IsNullOrEmpty(FilteredCard))
		{
			nameOverride = SokLoc.Translate(NameOverride, LocParam.Create("card", WorldManager.instance.GameDataLoader.GetCardFromId(FilteredCard).Name));
		}
		else
		{
			nameOverride = null;
		}
	}
}
