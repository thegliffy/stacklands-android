public class Milk : Food
{
	public override void StoppedDragging()
	{
		if (MyGameCard.HasParent && MyGameCard.Parent.CardData.Id == "feral_cat" && WorldManager.instance.IsSpiritDlcActive())
		{
			MyGameCard.Parent.DestroyCard();
			MyGameCard.DestroyCard();
			CardData cardData = WorldManager.instance.CreateCard(base.transform.position, "cat");
			WorldManager.instance.CreateSmoke(cardData.transform.position);
			cardData.MyGameCard.SendIt();
		}
		else
		{
			base.StoppedDragging();
		}
	}
}
