public class Rumor : CardData, IKnowledge
{
	public BlueprintGroup KnowledgeGroup;

	public BlueprintGroup Group => KnowledgeGroup;

	public string CardId => Id;

	public string KnowledgeName => base.FullName;

	public string KnowledgeText => base.Description;

	public bool IsIslandKnowledge
	{
		get
		{
			if (KnowledgeGroup != BlueprintGroup.Island)
			{
				return KnowledgeGroup == BlueprintGroup.Sailing;
			}
			return true;
		}
	}

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (!(otherCard is Blueprint))
		{
			return otherCard is Rumor;
		}
		return true;
	}
}
