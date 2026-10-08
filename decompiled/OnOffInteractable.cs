using Shapes;
using UnityEngine;

public class OnOffInteractable : Interactable
{
	public GameCard ParentCard;

	private Vector3 startScale;

	public Rectangle ButtonShape;

	public string TooltipTerm;

	public string gameObjectTerm;

	public override bool CanBeAutoMovedTo
	{
		get
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return false;
			}
			if (WorldManager.instance.DraggingCard != null)
			{
				return false;
			}
			if (!ParentCard.ShowInventory)
			{
				return !ParentCard.BeingDragged;
			}
			return false;
		}
	}

	public override string GetTooltipText()
	{
		return SokLoc.Translate(TooltipTerm);
	}

	public override void Clicked()
	{
		ParentCard.ToggleCardOnOff();
	}

	public override bool CanBeDragged()
	{
		return false;
	}

	protected override void Start()
	{
		startScale = base.transform.localScale;
		base.Start();
	}

	protected override void Update()
	{
		MyBoard = ParentCard.MyBoard;
		base.gameObject.name = SokLoc.Translate(gameObjectTerm);
		Vector3 b = (IsHovered ? (startScale * 1.1f) : startScale);
		base.transform.localScale = Vector3.Lerp(base.transform.localScale, b, Time.deltaTime * 12f);
		if (ParentCard.CardData != null)
		{
			if (ParentCard.CardData.IsOn)
			{
				ButtonShape.Color = ColorManager.instance.FloatingTextColorSuccess;
			}
			else
			{
				ButtonShape.Color = ColorManager.instance.FloatingTextColorFailed;
			}
		}
	}

	public override bool CanBePushed()
	{
		return false;
	}

	public override bool CanBePushedBy(Draggable draggable)
	{
		return false;
	}

	protected override void ClampPos()
	{
	}
}
