using System;
using Newtonsoft.Json;
using UnityEngine;

internal class StringEnumConverter : JsonConverter
{
	public override bool CanConvert(Type objectType)
	{
		return objectType.IsEnum;
	}

	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		throw new NotImplementedException();
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		try
		{
			return (Enum)Enum.ToObject(objectType, EnumHelper.ParseEnum(objectType, reader.Value.ToString()));
		}
		catch (Exception)
		{
			Debug.LogWarning($"Failed to parse enum ({objectType}) {reader.Value.ToString()}");
			return (Enum)Enum.ToObject(objectType, 0);
		}
	}
}
