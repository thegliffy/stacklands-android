using UnityEngine;

public class ShowTooltip : MonoBehaviour
{
	public string MyTooltipTerm;

	public string MyTooltipText;

	private void Update()
	{
		if (InputController.instance.CurrentScheme != ControlScheme.KeyboardMouse)
		{
			return;
		}
		GameObject mouseOverObject = GameCanvas.instance.MouseOverObject;
		if (mouseOverObject != null && (mouseOverObject.transform.IsChildOf(base.transform) || mouseOverObject.transform == base.transform))
		{
			if (!string.IsNullOrEmpty(MyTooltipTerm))
			{
				Tooltip.Text = SokLoc.Translate(MyTooltipTerm);
			}
			else
			{
				Tooltip.Text = MyTooltipText;
			}
		}
	}
}
