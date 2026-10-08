using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class Colorizer : MonoBehaviour
{
	public UIColor Color;

	private void OnValidate()
	{
		SetColors();
	}

	private void Start()
	{
		SetColors();
	}

	private void Update()
	{
		if (!Application.isPlaying)
		{
			SetColors();
		}
	}

	private void SetColors()
	{
		if (!(ColorManager.instance == null))
		{
			Image component = GetComponent<Image>();
			if (component != null)
			{
				component.color = ColorManager.instance.GetColor(Color);
			}
			TextMeshProUGUI component2 = GetComponent<TextMeshProUGUI>();
			if (component2 != null)
			{
				component2.color = ColorManager.instance.GetColor(Color);
			}
		}
	}
}
