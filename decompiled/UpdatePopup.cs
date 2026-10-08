using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpdatePopup : MonoBehaviour
{
	public TextMeshProUGUI UpdateText;

	public TextMeshProUGUI UpdateTitle;

	public CustomButton CloseUpdateInfoButton;

	public CustomButton BuyDLCButton;

	private void Awake()
	{
		CloseUpdateInfoButton.Clicked += delegate
		{
			if (PlatformHelper.IsTestBuild && WorldManager.instance.IsCitiesDlcActive())
			{
				GameCanvas.instance.ShowEarlyAccessModal();
			}
			base.gameObject.SetActive(value: false);
		};
		CloseUpdateInfoButton.ExplicitNavigationChanged += delegate(CustomButton cb, Navigation nav)
		{
			nav.selectOnUp = (BuyDLCButton.gameObject.activeInHierarchy ? BuyDLCButton : null);
			Selectable selectable5 = (nav.selectOnRight = null);
			Selectable selectOnDown = (nav.selectOnLeft = selectable5);
			nav.selectOnDown = selectOnDown;
			return nav;
		};
		BuyDLCButton.ExplicitNavigationChanged += delegate(CustomButton cb, Navigation nav)
		{
			Selectable selectable2 = (nav.selectOnRight = null);
			Selectable selectOnUp = (nav.selectOnLeft = selectable2);
			nav.selectOnUp = selectOnUp;
			nav.selectOnDown = CloseUpdateInfoButton;
			return nav;
		};
		BuyDLCButton.Clicked += delegate
		{
			SteamFriends.ActivateGameOverlayToWebPage("https://store.steampowered.com/app/2867570/Stacklands_2000");
		};
		UpdatePopupText();
	}

	private void OnEnable()
	{
		EventSystem.current.SetSelectedGameObject(CloseUpdateInfoButton.gameObject);
	}

	private void Update()
	{
		UpdatePopupText();
	}

	private void UpdatePopupText()
	{
		UpdateTitle.text = SokLoc.Translate("label_update_title_cities");
		if (WorldManager.instance.IsCitiesDlcActive())
		{
			UpdateText.text = SokLoc.Translate("label_update_text_cities");
			BuyDLCButton.gameObject.SetActive(value: false);
		}
		else
		{
			UpdateText.text = SokLoc.Translate("label_update_text_cities_locked");
			BuyDLCButton.gameObject.SetActive(value: true);
		}
	}
}
