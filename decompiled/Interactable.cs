public class Interactable : Draggable
{
	public virtual string GetTooltipText()
	{
		return "";
	}

	public virtual void Click()
	{
	}

	public override bool CanBeDragged()
	{
		return false;
	}

	public override bool CanBePushed()
	{
		return false;
	}

	public override bool CanBePushedBy(Draggable draggable)
	{
		return false;
	}
}
