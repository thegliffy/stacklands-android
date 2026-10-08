using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ModalScreen : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	public static ModalScreen instance;

	public CustomButton ButtonPrefab;

	public RectTransform ButtonParent;

	public TMP_InputField InputPrefab;

	public RectTransform InputParent;

	public TextMeshProUGUI TitleText;

	public TextMeshProUGUI TextText;

	private void Awake()
	{
		instance = this;
	}

	public void Clear()
	{
		foreach (RectTransform item in ButtonParent)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
		foreach (RectTransform item2 in InputParent)
		{
			UnityEngine.Object.Destroy(item2.gameObject);
		}
	}

	public void SetTexts(string title, string text)
	{
		TitleText.text = title;
		TextText.text = text;
	}

	public void AddOption(string text, Action action)
	{
		CustomButton customButton = UnityEngine.Object.Instantiate(ButtonPrefab);
		customButton.transform.SetParentClean(ButtonParent);
		customButton.TextMeshPro.text = text;
		customButton.Clicked += action;
	}

	public TMP_InputField AddInput(string confirmText, Action<string> action)
	{
		TMP_InputField input = UnityEngine.Object.Instantiate(InputPrefab);
		input.transform.SetParentClean(InputParent);
		CustomButton customButton = UnityEngine.Object.Instantiate(ButtonPrefab);
		customButton.transform.SetParentClean(ButtonParent);
		customButton.TextMeshPro.text = confirmText;
		customButton.Clicked += delegate
		{
			action(input.text);
		};
		return input;
	}

	public TMP_InputField AddInputNoButton()
	{
		TMP_InputField tMP_InputField = UnityEngine.Object.Instantiate(InputPrefab);
		tMP_InputField.transform.SetParentClean(InputParent);
		return tMP_InputField;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		int num = -1;
		if (Mouse.current != null)
		{
			num = TMP_TextUtilities.FindIntersectingLink(TextText, new Vector3(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue(), 0f), null);
		}
		if (num != -1)
		{
			TMP_LinkInfo tMP_LinkInfo = TextText.textInfo.linkInfo[num];
			if (tMP_LinkInfo.GetLinkID().StartsWith("https://"))
			{
				Debug.Log("Clicked '" + tMP_LinkInfo.GetLinkText() + ", opening '" + tMP_LinkInfo.GetLinkID() + "' in browser");
				Application.OpenURL(tMP_LinkInfo.GetLinkID());
			}
		}
	}
}
