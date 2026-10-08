public class Junction : CardData
{
	protected override bool CanHaveCard(CardData otherCard)
	{
		if (otherCard.MyCardType == CardType.Structures)
		{
			return false;
		}
		return true;
	}

	public bool AnyTransportConnected()
	{
		for (int i = 0; i < MyGameCard.CardConnectorChildren.Count; i++)
		{
			CardConnector cardConnector = MyGameCard.CardConnectorChildren[i];
			if (cardConnector.ConnectionType == ConnectionType.Transport && cardConnector.ConnectedNode != null)
			{
				return true;
			}
		}
		return false;
	}

	public override void UpdateCard()
	{
		if (MyGameCard.HasChild)
		{
			for (int num = MyGameCard.GetChildCards().Count - 1; num >= 0; num--)
			{
				GameCard gameCard = MyGameCard.GetChildCards()[num];
				gameCard.RemoveFromStack();
				if (AnyTransportConnected())
				{
					WorldManager.instance.StackSendCheckTarget(MyGameCard, gameCard, OutputDir);
				}
				else
				{
					gameCard.SendIt();
				}
			}
		}
		base.UpdateCard();
	}
}
