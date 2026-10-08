using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Conflict
{
	public List<Combatable> Participants = new List<Combatable>();

	public Combatable Initiator;

	public Vector3 ConflictStartPosition;

	public string Id = "";

	public float TimeSinceLastAttack;

	private float conflictTime;

	private float timeSinceLastHover;

	private Collider[] hits = new Collider[20];

	private bool init;

	private Vector3 currentPosition;

	private Vector2 currentSize;

	public static Conflict CreateFromSavedConflict(SavedConflict savedConflict)
	{
		Conflict conflict = new Conflict();
		conflict.Id = savedConflict.Id;
		conflict.ConflictStartPosition = savedConflict.StartPosition;
		GameCard cardWithUniqueId = WorldManager.instance.GetCardWithUniqueId(savedConflict.InitiatorCardId);
		if (cardWithUniqueId == null)
		{
			return null;
		}
		conflict.Initiator = cardWithUniqueId.Combatable;
		foreach (string involvedCard in savedConflict.InvolvedCards)
		{
			GameCard cardWithUniqueId2 = WorldManager.instance.GetCardWithUniqueId(involvedCard);
			if (cardWithUniqueId2 != null)
			{
				conflict.JoinConflict(cardWithUniqueId2.Combatable);
			}
		}
		return conflict;
	}

	public static Conflict StartConflict(Combatable initiator)
	{
		Conflict obj = new Conflict
		{
			Id = Guid.NewGuid().ToString().Substring(0, 10),
			Initiator = initiator
		};
		Vector3 position = initiator.MyGameCard.transform.position;
		obj.ConflictStartPosition = new Vector3(position.x, (0f - position.z) * 0.001f, position.z);
		obj.JoinConflict(initiator);
		return obj;
	}

	public bool CanLeaveConflict(Combatable b)
	{
		return Initiator.Team == b.Team;
	}

	public void JoinConflict(Combatable b)
	{
		if (b.InConflict)
		{
			Debug.LogError($"{b} is already in a conflict");
			return;
		}
		if (Participants.Contains(b))
		{
			Debug.LogError($"{b} is already part of this conflict");
			return;
		}
		if (b.MyGameCard.HasChild)
		{
			foreach (Combatable item in b.ChildrenMatchingPredicate((CardData x) => x is Combatable).Cast<Combatable>().ToList())
			{
				Participants.Add(item);
				item.MyConflict = this;
				item.MyGameCard.RemoveFromStack();
			}
		}
		Participants.Add(b);
		b.MyConflict = this;
		b.MyGameCard.RemoveFromStack();
	}

	public void SetParticipantTeamIndex(Combatable a, int index)
	{
		index = Mathf.Clamp(index, 0, GetTeamSize(a.Team));
		Combatable participantWithTeamIndex = GetParticipantWithTeamIndex(a.Team, index);
		if (!(participantWithTeamIndex == null))
		{
			int index2 = Participants.IndexOf(a);
			int index3 = Participants.IndexOf(participantWithTeamIndex);
			Participants[index2] = participantWithTeamIndex;
			Participants[index3] = a;
		}
	}

	private Combatable GetParticipantWithTeamIndex(Team team, int teamIndex)
	{
		for (int i = 0; i < Participants.Count; i++)
		{
			if (Participants[i].Team == team && GetIndexInTeam(Participants[i]) == teamIndex)
			{
				return Participants[i];
			}
		}
		return null;
	}

	public void LeaveConflict(Combatable b)
	{
		RemoveParticipant(b);
	}

	public void UpdateConflict()
	{
		conflictTime += Time.deltaTime;
		TimeSinceLastAttack += Time.deltaTime * WorldManager.instance.TimeScale;
		if (!BothTeamsExist())
		{
			StopConflict();
		}
		foreach (Conflict allConflict in WorldManager.instance.GetAllConflicts())
		{
			if (allConflict != this && OverlapsWith(allConflict))
			{
				Debug.Log("Joined conflicts because of overlap");
				JoinWithConflict(allConflict);
				break;
			}
		}
		UpdateConflictArrows();
		UpdateConflictOutline();
		PushDraggables();
	}

	public void PushDraggables()
	{
		Bounds bounds = GetBounds();
		int num = Physics.OverlapBoxNonAlloc(bounds.center, bounds.extents, hits);
		for (int i = 0; i < num; i++)
		{
			Draggable component = hits[i].gameObject.GetComponent<Draggable>();
			if (!(component == null) && component.CanBePushed())
			{
				Draggable draggable = component;
				if (draggable is GameCard gameCard)
				{
					draggable = gameCard.GetRootCard();
				}
				Vector3 vector = bounds.center - draggable.TargetPosition;
				vector.y = 0f;
				draggable.TargetPosition -= vector.normalized * 2f * Time.deltaTime;
			}
		}
	}

	private void UpdateConflictArrows()
	{
		bool flag = true;
		if (WorldManager.instance.SpeedUp != 0f)
		{
			flag = false;
		}
		bool flag2 = false;
		foreach (Combatable participant in Participants)
		{
			if (participant.MyGameCard.BeingHovered)
			{
				flag2 = true;
			}
		}
		if (flag2)
		{
			timeSinceLastHover = 0f;
		}
		else
		{
			timeSinceLastHover += Time.deltaTime;
		}
		if (timeSinceLastHover < 0.1f)
		{
			flag = false;
		}
		if (!flag)
		{
			return;
		}
		foreach (Combatable participant2 in Participants)
		{
			participant2.DrawConflictArrows(onlyVeryEffective: true);
		}
	}

	private void UpdateConflictOutline()
	{
		Bounds bounds = GetBounds();
		_ = Extensions.Perlin(conflictTime * 10f) * 0.01f;
		Vector2 vector = new Vector2(bounds.size.x, bounds.size.z);
		Vector3 center = bounds.center;
		if (!init)
		{
			init = true;
			currentPosition = center;
		}
		currentPosition = Vector3.Lerp(currentPosition, center, Time.deltaTime * 16f);
		currentSize = Vector3.Lerp(currentSize, vector, Time.deltaTime * 16f);
		DrawManager.instance.DrawShape(new ConflictRectangle
		{
			Size = currentSize,
			Center = currentPosition
		});
	}

	private void JoinWithConflict(Conflict otherConflict)
	{
		List<Combatable> list = new List<Combatable>(Participants);
		StopConflict();
		foreach (Combatable item in list)
		{
			otherConflict.JoinConflict(item);
		}
	}

	private bool OverlapsWith(Conflict otherConflict)
	{
		Bounds bounds = GetBounds();
		Bounds bounds2 = otherConflict.GetBounds();
		return bounds.Intersects(bounds2);
	}

	private void RemoveParticipant(Combatable b)
	{
		if (!Participants.Contains(b) || b.MyConflict != this)
		{
			Debug.LogError($"{b} is not part of this conflict");
			return;
		}
		Participants.Remove(b);
		b.MyConflict = null;
		b.ExitConflict();
		if (Initiator == b && Participants.Count > 0)
		{
			if (GetTeamSize(b.Team) > 0)
			{
				Initiator = GetCombatableWithIndexInTeam(b.Team, 0);
			}
			else
			{
				Initiator = GetCombatableWithIndexInTeam(GetOppositeTeam(b.Team), 0);
			}
			if (Initiator == null)
			{
				Debug.Log("Initiator is null");
			}
		}
	}

	public void SwapParticipant(Combatable oldParticipant, Combatable newParticipant)
	{
		int index = Participants.IndexOf(oldParticipant);
		oldParticipant.MyConflict.JoinConflict(newParticipant);
		oldParticipant.MyConflict.LeaveConflict(oldParticipant);
		Participants.Remove(newParticipant);
		Participants.Insert(index, newParticipant);
		foreach (Combatable participant in Participants)
		{
			participant.NotifyParticipantUpdate(oldParticipant, newParticipant);
		}
	}

	public void StopConflict()
	{
		for (int num = Participants.Count - 1; num >= 0; num--)
		{
			Combatable b = Participants[num];
			RemoveParticipant(b);
		}
	}

	public List<Combatable> GetFriendlyParticipants(Combatable combatable)
	{
		return Participants.FindAll((Combatable x) => x.Team == combatable.Team);
	}

	public List<Combatable> GetEnemyParticipants(Combatable combatable)
	{
		return Participants.FindAll((Combatable x) => x.Team == GetOppositeTeam(combatable.Team));
	}

	public Combatable GetTarget(Combatable b)
	{
		DetermineTargetRange(b, out var min, out var max);
		int index = UnityEngine.Random.Range(min, max);
		if (GetTeamSize(GetOppositeTeam(b.Team)) == 0)
		{
			return null;
		}
		if (b.Team == Team.Player && max - min > 1 && UnityEngine.Random.value > 0.5f)
		{
			Combatable combatable = GetCombatableWithIndexInTeam(GetOppositeTeam(b.Team), min);
			for (int i = min; i <= max - 1; i++)
			{
				Combatable combatableWithIndexInTeam = GetCombatableWithIndexInTeam(GetOppositeTeam(b.Team), i);
				if (combatableWithIndexInTeam.HealthPoints < combatable.HealthPoints)
				{
					combatable = combatableWithIndexInTeam;
				}
			}
			index = combatable.MyConflict.GetIndexInTeam(combatable);
		}
		return GetCombatableWithIndexInTeam(GetOppositeTeam(b.Team), index);
	}

	public List<Combatable> GetCombatableTargets(Combatable b)
	{
		List<Combatable> list = new List<Combatable>();
		DetermineTargetRange(b, out var min, out var max);
		if (GetTeamSize(GetOppositeTeam(b.Team)) == 0)
		{
			return list;
		}
		for (int i = min; i < max; i++)
		{
			list.Add(GetCombatableWithIndexInTeam(GetOppositeTeam(b.Team), i));
		}
		return list;
	}

	private void DetermineTargetRange(Combatable b, out int min, out int max)
	{
		float num = GetTeamSize(b.Team);
		float num2 = GetIndexInTeam(b);
		float num3 = (float)GetTeamSize(GetOppositeTeam(b.Team)) / num;
		float f = num2 * num3;
		float f2 = (num2 + 1f) * num3;
		min = Mathf.FloorToInt(f);
		max = Mathf.CeilToInt(f2);
	}

	public int GetIndexInTeam(Combatable b)
	{
		int num = 0;
		foreach (Combatable participant in Participants)
		{
			if (participant == b)
			{
				return num;
			}
			if (participant.Team == b.Team)
			{
				num++;
			}
		}
		return -1;
	}

	private Combatable GetCombatableWithIndexInTeam(Team team, int index)
	{
		if (index < 0 || index >= GetTeamSize(team))
		{
			throw new ArgumentOutOfRangeException("index");
		}
		int num = 0;
		foreach (Combatable participant in Participants)
		{
			if (participant.Team == team && index == num)
			{
				return participant;
			}
			if (participant.Team == team)
			{
				num++;
			}
		}
		return null;
	}

	private Team GetOppositeTeam(Team team)
	{
		if (team == Team.Enemy)
		{
			return Team.Player;
		}
		return Team.Enemy;
	}

	public int GetTeamSize(Team team)
	{
		int num = 0;
		foreach (Combatable participant in Participants)
		{
			if (participant.Team == team)
			{
				num++;
			}
		}
		return num;
	}

	public bool BothTeamsExist()
	{
		if (GetTeamSize(Team.Player) > 0)
		{
			return GetTeamSize(Team.Enemy) > 0;
		}
		return false;
	}

	public Bounds GetBounds()
	{
		float combatOffset = WorldManager.instance.CombatOffset;
		return new Bounds(ClampStartPosition(ConflictStartPosition) - new Vector3(0f, 0f, combatOffset) * 0.5f, GetConflictSize());
	}

	private Vector3 GetConflictSize()
	{
		float num = (float)Mathf.Max(GetTeamSize(Team.Player), GetTeamSize(Team.Enemy)) * WorldManager.instance.HorizonalCombatOffset;
		float height = Initiator.MyGameCard.GetHeight();
		return new Vector3(z: height + WorldManager.instance.CombatOffset + WorldManager.instance.ConflictHeightIncrease, x: num + WorldManager.instance.ConflictWidthIncrease, y: 0.05f);
	}

	public static float GetConflictHeight()
	{
		return WorldManager.instance.CombatOffset + GameCard.CardHeight;
	}

	private Vector3 ClampStartPosition(Vector3 p)
	{
		Vector3 conflictSize = GetConflictSize();
		float num = conflictSize.x * 0.5f;
		float num2 = conflictSize.z * 0.5f;
		Bounds tightWorldBounds = Initiator.MyGameCard.MyBoard.TightWorldBounds;
		float num3 = 0.1f;
		p.x = Mathf.Clamp(p.x, tightWorldBounds.min.x + num + num3, tightWorldBounds.max.x - num - num3);
		p.z = Mathf.Clamp(p.z, tightWorldBounds.min.z + num2 * 0.5f + num3 + WorldManager.instance.CombatOffset, tightWorldBounds.max.z + num2 * 0.5f - num3);
		return p;
	}

	public Vector3 GetPositionInConflict(Combatable b)
	{
		float num = GetTeamSize(b.Team);
		float num2 = (float)GetIndexInTeam(b) - (num - 1f) * 0.5f;
		Vector3 vector = new Vector3(num2 * WorldManager.instance.HorizonalCombatOffset, 0f, 0f);
		Vector3 vector2 = ClampStartPosition(ConflictStartPosition);
		if (Initiator.Team != b.Team)
		{
			return vector2 + vector;
		}
		return vector2 + vector + new Vector3(0f, 0f, 0f - WorldManager.instance.CombatOffset);
	}
}
