using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardopediaEntryElement : MonoBehaviour
{
	public CustomButton Button;

	[HideInInspector]
	public CardData MyCardData;

	public RectTransform NewTextTransform;

	public RectTransform NewBackgroundTransform;

	public RectTransform UndiscoveredTransform;

	public RectTransform UpdateTypeTransform;

	public ShowTooltip tooltip;

	public Image UpdateImage;

	public Sprite MainIcon;

	public Sprite SpiritIcon;

	public Sprite ForestIcon;

	public Sprite IslandIcon;

	public Sprite ModIcon;

	public Sprite OrderIcon;

	public Sprite CitiesIcon;

	public bool wasFound;

	public bool IsFiltered;

	public bool IsFilteredUpdate;

	public bool HasUndiscoveredCards;

	public bool IsNew;

	public bool IsEnabled;

	private bool wasHoveredAndNew;

	public List<CanvasRenderer> CanvasRenderers;

	private bool isCulled;

	public void SetCardData(CardData cardData)
	{
		MyCardData = cardData;
		wasFound = WorldManager.instance.CurrentSave.FoundCardIds.Contains(cardData.Id) || (DebugOptions.Default.UnlockAllInCardopedia && Application.isEditor);
		IsNew = WorldManager.instance.CurrentSave.NewCardopediaIds.Contains(cardData.Id);
		HasUndiscoveredCards = cardData.HasUndiscoveredCardInDrops() && wasFound;
		UpdateUndiscoveredCardsIcon();
		UpdateIsNew();
		if (cardData.CardUpdateType == CardUpdateType.Spirit)
		{
			UpdateImage.sprite = SpiritIcon;
			tooltip.MyTooltipTerm = "label_cardopedia_spirit";
		}
		else if (cardData.CardUpdateType == CardUpdateType.Forest)
		{
			UpdateImage.sprite = ForestIcon;
			tooltip.MyTooltipTerm = "label_cardopedia_forest";
		}
		else if (cardData.CardUpdateType == CardUpdateType.Island)
		{
			UpdateImage.sprite = IslandIcon;
			tooltip.MyTooltipTerm = "label_cardopedia_island";
		}
		else if (cardData.CardUpdateType == CardUpdateType.Order)
		{
			UpdateImage.sprite = OrderIcon;
			tooltip.MyTooltipTerm = "label_cardopedia_order";
		}
		else if (cardData.CardUpdateType == CardUpdateType.Cities)
		{
			UpdateImage.sprite = CitiesIcon;
			tooltip.MyTooltipTerm = "label_cardopedia_cities";
		}
		else if (cardData.CardUpdateType == CardUpdateType.Mod)
		{
			UpdateImage.sprite = ModIcon;
			tooltip.MyTooltipTerm = "label_cardopedia_modded";
		}
		else
		{
			UpdateImage.sprite = MainIcon;
			tooltip.MyTooltipTerm = "label_cardopedia_main";
		}
		UpdateText();
	}

	private void UpdateUndiscoveredCardsIcon()
	{
		UndiscoveredTransform.gameObject.SetActive(HasUndiscoveredCards && !IsNew);
	}

	private void Update()
	{
		UpdateIsNew();
		if (Button.IsHovered || Button.IsSelected)
		{
			if (IsNew)
			{
				wasHoveredAndNew = true;
			}
		}
		else if (wasHoveredAndNew)
		{
			wasHoveredAndNew = false;
			IsNew = false;
			WorldManager.instance.CurrentSave.NewCardopediaIds.Remove(MyCardData.Id);
			SaveManager.instance.Save(saveRound: false);
			UpdateUndiscoveredCardsIcon();
		}
	}

	private void UpdateIsNew()
	{
		NewTextTransform.gameObject.SetActive(IsNew);
		NewBackgroundTransform.gameObject.SetActive(IsNew);
		if (IsNew)
		{
			NewBackgroundTransform.sizeDelta = new Vector2(NewTextTransform.rect.width, NewTextTransform.rect.height);
			NewBackgroundTransform.position = NewTextTransform.position;
		}
	}

	public void Cull(bool cull)
	{
		if (isCulled != cull)
		{
			isCulled = cull;
			for (int i = 0; i < CanvasRenderers.Count; i++)
			{
				CanvasRenderers[i].cull = cull;
			}
			if (!isCulled)
			{
				NewBackgroundTransform.sizeDelta = new Vector2(NewTextTransform.rect.width + 0.001f, NewTextTransform.rect.height);
				UpdateImage.rectTransform.sizeDelta = new Vector2(UpdateImage.rectTransform.rect.width + 0.001f, UpdateImage.rectTransform.rect.height);
				UndiscoveredTransform.sizeDelta = new Vector2(UndiscoveredTransform.rect.width + 0.001f, UndiscoveredTransform.rect.height);
			}
		}
	}

	public void UpdateText()
	{
		if (MyCardData != null)
		{
			MyCardData.UpdateCardText();
		}
		if (wasFound)
		{
			Button.TextMeshPro.text = "• " + MyCardData.Name;
		}
		else
		{
			Button.TextMeshPro.text = "• ???";
		}
	}
}
