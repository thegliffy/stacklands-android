using UnityEngine;

public class Spirit : CardData
{
	public AudioClip CreateSound;

	public int MaxCapacity = 10;

	public override bool DetermineCanHaveCardsWhenIsRoot => true;

	public bool IsReturning
	{
		get
		{
			if (MyGameCard.TimerRunning)
			{
				return MyGameCard.TimerActionId == GetActionId("LeaveWithSpirit");
			}
			return false;
		}
	}

	private bool finishedSpiritRun
	{
		get
		{
			if ((WorldManager.instance.CurrentBoard.Id == "happiness" && WorldManager.instance.CurrentSave.FinishedHappiness) || (WorldManager.instance.CurrentBoard.Id == "greed" && WorldManager.instance.CurrentSave.FinishedGreed) || (WorldManager.instance.CurrentBoard.Id == "death" && WorldManager.instance.CurrentSave.FinishedDeath))
			{
				return true;
			}
			return false;
		}
	}

	public override void OnInitialCreate()
	{
		AudioManager.me.PlaySound2D(CreateSound, 1f, 0.5f);
		base.OnInitialCreate();
	}

	protected override void Awake()
	{
		base.Awake();
	}

	public override void UpdateCard()
	{
		base.UpdateCard();
		SpiritMovement();
		TryReturnToMainland();
	}

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (GetChildCount() + (otherCard.GetChildCount() + 1) > MaxCapacity)
		{
			return false;
		}
		if (!(otherCard is Enemy))
		{
			return !(otherCard is Harvestable);
		}
		return false;
	}

	public override bool CanHaveCardsWhileHasStatus()
	{
		return true;
	}

	public void TryReturnToMainland()
	{
		if (MyGameCard.HasChild && finishedSpiritRun)
		{
			if (!IsReturning)
			{
				MyGameCard.StartTimer(30f, LeaveWithSpirit, SokLoc.Translate("card_spirit_status_1"), GetActionId("LeaveWithSpirit"));
			}
		}
		else
		{
			MyGameCard.CancelTimer(GetActionId("LeaveWithSpirit"));
		}
	}

	[TimedAction("leave_spirit")]
	public void LeaveWithSpirit()
	{
		if (!TransitionScreen.InTransition && !WorldManager.instance.InAnimation)
		{
			GameCanvas.instance.LeaveSpiritWorldPrompt(ReturnToMainland, Stay);
		}
	}

	public void ReturnToMainland()
	{
		GameBoard targetBoard = WorldManager.instance.GetBoardWithId(WorldManager.instance.CurrentRunVariables.PreviouseBoard);
		GameBoard board = WorldManager.instance.CurrentBoard;
		WorldManager.instance.GoToBoard(targetBoard, delegate
		{
			GameCanvas.instance.SetScreen<GameScreen>();
			GameCard child = MyGameCard.Child;
			child.RemoveFromParent();
			WorldManager.instance.SendStackToBoard(child, targetBoard, new Vector2(0.4f, 0.5f));
			WorldManager.instance.RemoveAllCardsFromBoard(board.Id);
			WorldManager.instance.ResetBoughtBoostersOnLocation(board.Location);
			if (board.Id == "greed")
			{
				DemandManager.instance.ResetDemands();
				WorldManager.instance.BoardMonths.GreedMonth = 1;
			}
			if (board.Id == "happiness")
			{
				WorldManager.instance.CurrentRunVariables.VillagersUnhappyMonthCount = 0;
				WorldManager.instance.CurrentRunVariables.VillagersHappyMonthCount = 0;
				WorldManager.instance.BoardMonths.HappinessMonth = 1;
			}
			if (board.Id == "death")
			{
				WorldManager.instance.BoardMonths.DeathMonth = 1;
			}
		}, "spirit");
	}

	public void CreateBackgroundPlane()
	{
		GameObject obj = Object.Instantiate(PrefabManager.instance.SpiritBackgroundPlanePrefab);
		obj.transform.SetParent(MyGameCard.Visuals);
		obj.transform.localPosition = Vector3.zero;
		obj.GetComponent<MeshRenderer>().material = GameCamera.instance.TempSpiritBackgroundMaterial;
		MyGameCard.MinY = 0.25f;
	}

	public override void OnDestroyCard()
	{
		WorldManager.instance.CreateSmoke(base.transform.position);
		base.OnDestroyCard();
	}

	public void Stay()
	{
		MyGameCard.RemoveFromStack();
	}

	private void SpiritMovement()
	{
		MyGameCard.TargetPosition += Vector3.left * 0.001f * Mathf.Cos(Time.time);
		MyGameCard.TargetPosition += Vector3.forward * 0.0005f * Mathf.Cos(Time.time * 0.5f);
	}
}
