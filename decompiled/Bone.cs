public class Bone : Resource
{
	public override void StoppedDragging()
	{
		if (MyGameCard.HasParent && MyGameCard.Parent.CardData.Id == "wolf")
		{
			MyGameCard.Parent.DestroyCard();
			MyGameCard.DestroyCard();
			CardData cardData = WorldManager.instance.CreateCard(base.transform.position, "dog");
			WorldManager.instance.CreateSmoke(cardData.transform.position);
			cardData.MyGameCard.SendIt();
		}
		else
		{
			base.StoppedDragging();
		}
	}
}
