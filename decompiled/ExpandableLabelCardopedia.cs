using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExpandableLabelCardopedia : MonoBehaviour
{
	public CustomButton MyButton;

	public Image PlusImage;

	public TextMeshProUGUI LabelText;

	public Sprite PlusSprite;

	public Sprite MinusSprite;

	public List<CardopediaEntryElement> Children = new List<CardopediaEntryElement>();

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
			this.OnExpand?.Invoke();
		};
		MyButton.SetColor = false;
	}

	public void SetExpanded(bool expanded)
	{
		IsExpanded = expanded;
		foreach (CardopediaEntryElement child in Children)
		{
			child.IsEnabled = expanded;
		}
	}

	public void ShowChildrenCardopedia()
	{
		foreach (CardopediaEntryElement child in Children)
		{
			if (!CardopediaScreen.instance.IsSearching)
			{
				child.IsEnabled = IsExpanded && child.IsFilteredUpdate;
			}
			else
			{
				child.IsEnabled = child.wasFound && child.IsFiltered && IsExpanded;
			}
		}
	}

	private void Update()
	{
		PlusImage.sprite = ((!IsExpanded) ? PlusSprite : MinusSprite);
	}
}
