using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

internal class StringSpriteConverter : JsonConverter
{
	public override bool CanConvert(Type objectType)
	{
		return objectType == typeof(Sprite);
	}

	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		throw new NotImplementedException();
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		try
		{
			return ResourceHelper.LoadSpriteFromPath(Path.Combine(GameDataLoader.instance.CurrentlyLoadingMod.Path, "Icons", reader.Value.ToString()));
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			Debug.LogWarning($"Failed to read sprite from path Icons/{reader.Value}");
			return null;
		}
	}
}
