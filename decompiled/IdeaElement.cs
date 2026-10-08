using TMPro;
using UnityEngine;

public class IdeaElement : MonoBehaviour
{
	public CustomButton MyButton;

	[HideInInspector]
	public IKnowledge MyKnowledge;

	public TextMeshProUGUI MyText;

	public GameObject NewLabel;

	public bool IsNew;

	private void Update()
	{
		NewLabel.gameObject.SetActive(IsNew);
		if (MyButton.IsHovered || MyButton.IsSelected)
		{
			if (IsNew)
			{
				WorldManager.instance.CurrentSave.NewKnowledgeIds.Remove(MyKnowledge.CardId);
				IsNew = false;
			}
			GameScreen.InfoBoxTitle = MyKnowledge.KnowledgeName;
			GameScreen.InfoBoxText = MyKnowledge.KnowledgeText;
		}
		MyButton.Image.color = ColorManager.instance.BackgroundColor;
	}

	public void SetKnowledge(IKnowledge knowledge)
	{
		MyKnowledge = knowledge;
		IsNew = WorldManager.instance.CurrentSave.NewKnowledgeIds.Contains(MyKnowledge.CardId);
		MyText.text = "• " + MyKnowledge.KnowledgeName;
	}
}
