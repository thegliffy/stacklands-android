public class Trash : Harvestable
{
	public override ICardId GetCardToGive()
	{
		if (!WorldManager.instance.CurrentRunVariables.OpenedFirstTrash)
		{
			return new CardId("muesli");
		}
		return base.GetCardToGive();
	}

	public override void OnHarvestComplete()
	{
		WorldManager.instance.CurrentRunVariables.OpenedFirstTrash = true;
	}
}
