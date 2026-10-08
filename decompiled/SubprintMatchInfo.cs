public struct SubprintMatchInfo
{
	public int FullyMatchedAt;

	public int MatchCount;

	public SubprintMatchInfo(int matchedAt, int matchCount)
	{
		FullyMatchedAt = matchedAt;
		MatchCount = matchCount;
	}
}
