using UnityEngine;
using UnityEngine.UI;

public class MultilineScrollbarHider : MonoBehaviour
{
	public Scrollbar Scrollbar;

	private void Update()
	{
		bool active = Scrollbar.size < 1f || Scrollbar.value != 0f;
		Scrollbar.gameObject.SetActive(active);
	}
}
