using Shapes;
using TMPro;
using UnityEngine;

public class SellBox : CardTarget
{
	public Transform GoldSpawnPosition;

	public SpriteRenderer ImageSpriteRenderer;

	public Rectangle HighlightRectangle;

	public TextMeshPro SellText;

	public override void CardDropped(GameCard card)
	{
		WorldManager.instance.SellCard(GoldSpawnPosition.position, card);
		base.CardDropped(card);
	}

	public override bool CanHaveCard(GameCard card)
	{
		if (!WorldManager.instance.CardCanBeSold(card))
		{
			return false;
		}
		return true;
	}

	protected override void Update()
	{
		ImageSpriteRenderer.sprite = WorldManager.instance.GetCurrencyIcon(WorldManager.instance.CurrentBoard?.BoardOptions?.Currency);
		if (WorldManager.instance.CurrentBoard != null)
		{
			SellText.text = SokLoc.Translate(WorldManager.instance.CurrentBoard.BoardOptions.SellBoxTerm);
			base.gameObject.name = SokLoc.Translate(WorldManager.instance.CurrentBoard.BoardOptions.SellBoxTerm);
		}
		else
		{
			SellText.text = SokLoc.Translate("label_sell");
			base.gameObject.name = SokLoc.Translate("label_sellbox_title");
		}
		if (WorldManager.instance.CurrentBoard != null)
		{
			HighlightRectangle.Color = WorldManager.instance.CurrentBoard.CardHighlightColor;
		}
		HighlightRectangle.enabled = WorldManager.instance.DraggingCard != null && CanHaveCard(WorldManager.instance.DraggingCard);
		HighlightRectangle.DashOffset += Time.deltaTime;
		if (HighlightRectangle.DashOffset >= 1f)
		{
			HighlightRectangle.DashOffset -= 1f;
		}
		base.Update();
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();
		Vector3 localPosition = base.transform.localPosition;
		localPosition.z = 0f;
		base.transform.localPosition = localPosition;
	}

	public override string GetTooltipText()
	{
		if (WorldManager.instance.CurrentBoard != null)
		{
			return SokLoc.Translate(WorldManager.instance.CurrentBoard.BoardOptions.SellBoxDescription);
		}
		return SokLoc.Translate("label_sellbox_description", Extensions.LocParam_Action("sell"));
	}
}
