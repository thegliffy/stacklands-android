public interface IKnowledge
{
	string CardId { get; }

	bool IsIslandKnowledge { get; }

	string KnowledgeName { get; }

	string KnowledgeText { get; }

	BlueprintGroup Group { get; }
}
