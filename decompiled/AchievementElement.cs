using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementElement : MonoBehaviour
{
	public Quest MyQuest;

	public CustomButton MyButton;

	public Image Checkmark;

	public Image Checkbox;

	public TextMeshProUGUI AchievementNameText;

	public GameObject NewLabel;

	private bool isVisible;

	private bool isComplete;

	private bool isNew;

	public bool IsNew => isNew;

	private void Update()
	{
		UpdateVisuals();
	}

	private void UpdateVisuals()
	{
		if (!isVisible && !isComplete)
		{
			AchievementNameText.text = "???";
		}
		else
		{
			AchievementNameText.text = MyQuest.Description;
		}
		MyButton.Image.color = ColorManager.instance.BackgroundColor;
		if (MyButton.IsHovered || MyButton.IsSelected)
		{
			if (isNew || isComplete)
			{
				if (!WorldManager.instance.CurrentSave.SeenQuestIds.Contains(MyQuest.Id))
				{
					WorldManager.instance.CurrentSave.SeenQuestIds.Add(MyQuest.Id);
				}
				isNew = false;
			}
			if (isVisible || isComplete)
			{
				GameScreen.InfoBoxTitle = SokLoc.Translate("label_quest");
				GameScreen.InfoBoxText = MyQuest.Description;
			}
			else
			{
				GameScreen.InfoBoxTitle = SokLoc.Translate("label_quest");
				GameScreen.InfoBoxText = SokLoc.Translate("label_quests_complete_more_to_see");
			}
		}
		AchievementNameText.color = (isComplete ? ColorManager.instance.DisabledColor : Color.black);
		Checkmark.color = AchievementNameText.color;
		Checkbox.color = ColorManager.instance.DisabledColor;
		if (isVisible && !MyQuest.PossibleInPeacefulMode && WorldManager.instance.CurrentRunOptions.IsPeacefulMode)
		{
			MyButton.TooltipText = SokLoc.Translate("label_quest_not_possible_in_peaceful");
		}
		else
		{
			MyButton.TooltipText = "";
		}
		NewLabel.gameObject.SetActive(isNew);
		Checkmark.gameObject.SetActive(isComplete);
	}

	public void SetQuest(Quest ach)
	{
		MyQuest = ach;
		isComplete = QuestManager.instance.QuestIsComplete(MyQuest);
		isVisible = QuestManager.instance.QuestIsVisible(MyQuest);
		isNew = !WorldManager.instance.CurrentSave.SeenQuestIds.Contains(MyQuest.Id) && !isComplete;
		UpdateVisuals();
	}
}
