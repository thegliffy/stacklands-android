using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class ExtraDataAttribute : Attribute
{
	public string Identifier;

	public ExtraDataAttribute(string id)
	{
		Identifier = id;
	}
}
