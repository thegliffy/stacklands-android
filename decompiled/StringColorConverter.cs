using System;
using Newtonsoft.Json;
using UnityEngine;

internal class StringColorConverter : JsonConverter
{
	public override bool CanConvert(Type objectType)
	{
		return objectType == typeof(Color);
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		if (ColorUtility.TryParseHtmlString((string)reader.Value, out var color))
		{
			return color;
		}
		Debug.LogWarning($"Failed to parse color \"{reader.Value}\"");
		return Color.black;
	}

	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		writer.WriteValue(ColorUtility.ToHtmlStringRGBA((Color)value));
	}
}
