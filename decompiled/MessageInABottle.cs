public class MessageInABottle : Harvestable
{
	protected override void Emptied()
	{
		QuestManager.instance.SpecialActionComplete("open_message_in_bottle");
		base.Emptied();
	}
}
