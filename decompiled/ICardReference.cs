public interface ICardReference
{
	string ReferencedCardId { get; set; }

	string GetKey();
}
