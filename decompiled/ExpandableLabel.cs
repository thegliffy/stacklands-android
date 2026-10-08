using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExpandableLabel : MonoBehaviour
{
	public CustomButton MyButton;

	public Image PlusImage;

	public TextMeshProUGUI LabelText;

	public Sprite PlusSprite;

	public Sprite MinusSprite;

	public List<GameObject> Children = new List<GameObject>();

	public bool IsExpanded = true;

	public object Tag;

	public event Action OnExpand;

	public void SetText(string text)
	{
		LabelText.text = text;
	}

	public void SetCallback(Action callback)
	{
		OnExpand += callback;
	}

	private void Start()
	{
		MyButton.Clicked += delegate
		{
			SetExpanded(!IsExpanded);
			if (IsExpanded)
			{
				this.OnExpand?.Invoke();
			}
		};
		MyButton.SetColor = false;
	}

	public void SetExpanded(bool expanded)
	{
		IsExpanded = expanded;
		foreach (GameObject child in Children)
		{
			child.gameObject.SetActive(expanded);
		}
	}

	private void Update()
	{
		PlusImage.sprite = ((!IsExpanded) ? PlusSprite : MinusSprite);
	}
}
