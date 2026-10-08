using System.Collections.Generic;
using System.Linq;

public static class SerializedKeyValuePairHelper
{
	public static SerializedKeyValuePair GetWithKey(this List<SerializedKeyValuePair> values, string key)
	{
		return values.FirstOrDefault((SerializedKeyValuePair x) => x.Key == key);
	}

	public static void SetOrAdd(this List<SerializedKeyValuePair> values, string key, string value)
	{
		SerializedKeyValuePair withKey = values.GetWithKey(key);
		if (withKey != null)
		{
			withKey.Value = value;
			return;
		}
		values.Add(new SerializedKeyValuePair
		{
			Key = key,
			Value = value
		});
	}
}
