using UnityEngine;

public class ValidationError
{
	public Object Context;

	public string Error;

	public string Category;

	public ValidationError(Object context, string error, string category)
	{
		Context = context;
		Error = error;
		Category = category;
	}
}
