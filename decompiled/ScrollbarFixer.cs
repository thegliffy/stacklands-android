using UnityEngine;
using UnityEngine.UI;

public class ScrollbarFixer : MonoBehaviour
{
	private Scrollbar scrollbar;

	private void Start()
	{
		scrollbar = GetComponent<Scrollbar>();
	}

	private void Update()
	{
		scrollbar.interactable = !InputController.instance.CurrentSchemeIsController;
	}
}
