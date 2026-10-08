using UnityEngine;

public class ShowInfoBox : MonoBehaviour
{
	public string InfoBoxTitle;

	public string InfoBoxText;

	private RectTransform rectTransform;

	private void Start()
	{
		rectTransform = GetComponent<RectTransform>();
	}

	private void Update()
	{
		if (InputController.instance.CurrentScheme == ControlScheme.KeyboardMouse)
		{
			if (GameCanvas.instance.AboveMeOrMyChildren(rectTransform, GameCanvas.instance.MouseOverObject))
			{
				GameScreen.InfoBoxTitle = InfoBoxTitle;
				GameScreen.InfoBoxText = InfoBoxText;
			}
		}
		else if (InputController.instance.CurrentSchemeIsController && GameCanvas.instance.SelectedObject == base.gameObject)
		{
			GameScreen.InfoBoxTitle = InfoBoxTitle;
			GameScreen.InfoBoxText = InfoBoxText;
		}
	}
}
