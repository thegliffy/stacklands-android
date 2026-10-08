public interface HousingConsumer
{
	Apartment Housing { get; set; }

	string HousingId { get; }

	GameCard GetGameCard();

	int GetHousingSpaceRequired();

	WorkerType GetWorkerType();
}
