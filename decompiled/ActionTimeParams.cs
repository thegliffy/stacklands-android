public class ActionTimeParams
{
	public BaseVillager villager;

	public string actionId;

	public CardData baseCard;

	public ActionTimeParams(BaseVillager villager, string actionId, CardData baseCard)
	{
		this.villager = villager;
		this.actionId = actionId;
		this.baseCard = baseCard;
	}
}
