using System;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class TimedActionAttribute : Attribute
{
	public string Identifier;

	public TimedActionAttribute(string id)
	{
		Identifier = id;
	}
}
