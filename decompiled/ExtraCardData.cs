using System;
using UnityEngine;

[Serializable]
public class ExtraCardData
{
	public string AttributeId;

	public string StringValue;

	public int IntValue;

	public float FloatValue;

	public Vector3 VectorValue;

	public bool BoolValue;

	public ExtraCardData(string attributeId, string value)
	{
		AttributeId = attributeId;
		StringValue = value;
	}

	public ExtraCardData(string attributeId, int value)
	{
		AttributeId = attributeId;
		IntValue = value;
	}

	public ExtraCardData(string attributeId, float value)
	{
		AttributeId = attributeId;
		FloatValue = value;
	}

	public ExtraCardData(string attributeId, Vector3 value)
	{
		AttributeId = attributeId;
		VectorValue = value;
	}

	public ExtraCardData(string attributeId, bool value)
	{
		AttributeId = attributeId;
		BoolValue = value;
	}

	public override string ToString()
	{
		return $"{AttributeId} - {StringValue} - {IntValue} - {FloatValue}";
	}
}
