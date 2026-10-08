using System.Collections.Generic;

public static class IntToStringCacher
{
	public static Dictionary<int, string> cache = new Dictionary<int, string>();

	public static string ToStringCached(this int i)
	{
		if (!cache.TryGetValue(i, out var value))
		{
			cache[i] = i.ToString();
			return cache[i];
		}
		return value;
	}
}
