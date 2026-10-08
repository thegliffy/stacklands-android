using System.Collections.Generic;
using UnityEngine;

public class ValidationResult
{
	public List<ValidationError> errors = new List<ValidationError>();

	public string ValidationId = "";

	public void AddError(Object context, string error, string category)
	{
		errors.Add(new ValidationError(context, error, category));
	}
}
