public class CardReferenceCard : ICardReference
{
	public string OriginCardId;

	public string ReferencedCardId { get; set; }

	public CardReferenceCard(string referencedCardId, string originCardId)
	{
		OriginCardId = originCardId;
		ReferencedCardId = referencedCardId;
	}

	public override string ToString()
	{
		return "card " + OriginCardId;
	}

	public string GetKey()
	{
		return "card_" + OriginCardId + "_" + ReferencedCardId;
	}
}
