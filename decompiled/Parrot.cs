public class Parrot : Animal
{
	public override void StoppedDragging()
	{
		if (MyGameCard.HasParent && MyGameCard.Parent.CardData.Id == "pirate")
		{
			CardData cardData = WorldManager.instance.ChangeToCard(MyGameCard.Parent, "friendly_pirate");
			MyGameCard.DestroyCard();
			WorldManager.instance.CreateSmoke(cardData.transform.position);
			cardData.MyGameCard.SendIt();
			QuestManager.instance.SpecialActionComplete("befriend_pirate");
		}
		else
		{
			base.StoppedDragging();
		}
	}
}
