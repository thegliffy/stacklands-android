public class CardReferenceBooster : ICardReference
{
	public string BoosterId;

	public string ReferencedCardId { get; set; }

	public CardReferenceBooster(string referencedCardId, string boosterId)
	{
		ReferencedCardId = referencedCardId;
		BoosterId = boosterId;
	}

	public string GetKey()
	{
		return "booster_" + BoosterId + "_" + ReferencedCardId;
	}

	public override string ToString()
	{
		return "booster " + BoosterId;
	}
}
