using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TransitionScreen : MonoBehaviour
{
	private enum TransitionState
	{
		None,
		Entering,
		Holding,
		Leaving
	}

	public static TransitionScreen instance;

	public static bool InTransition;

	public static bool InRealTransition;

	public Image TransitionImage;

	public TransitionType CurrentTransitionType;

	private TransitionState currentState;

	private float transitionAmount;

	private float holdTimer;

	public List<TransitionType> TransitionTypes = new List<TransitionType>();

	private Action onTransition;

	private float wantedHoldTime;

	public bool IsLeaving => currentState == TransitionState.Leaving;

	public float TransitionAmount => transitionAmount;

	private void Awake()
	{
		instance = this;
		foreach (TransitionType transitionType in TransitionTypes)
		{
			transitionType.Material = new Material(transitionType.Material);
			transitionType.Material.SetFloat("_TransitionAmount", transitionAmount);
		}
		CurrentTransitionType = TransitionTypes[0];
		TransitionImage.material = CurrentTransitionType.Material;
		if (InTransition)
		{
			transitionAmount = 1f;
			currentState = TransitionState.Leaving;
		}
		else
		{
			transitionAmount = 0f;
			currentState = TransitionState.None;
		}
	}

	private void Update()
	{
		InRealTransition = false;
		if (InTransition)
		{
			if (currentState == TransitionState.Entering)
			{
				InRealTransition = true;
				transitionAmount += Time.unscaledDeltaTime * CurrentTransitionType.TransitionSpeed;
				if (transitionAmount >= 1f)
				{
					currentState = TransitionState.Holding;
				}
			}
			else if (currentState == TransitionState.Holding)
			{
				InRealTransition = true;
				holdTimer += Time.unscaledDeltaTime;
				if (holdTimer >= wantedHoldTime)
				{
					transitionAmount = 1.4f;
					InRealTransition = false;
					currentState = TransitionState.Leaving;
					if (onTransition != null)
					{
						onTransition();
					}
				}
			}
			else if (currentState == TransitionState.Leaving)
			{
				InRealTransition = false;
				transitionAmount -= Time.unscaledDeltaTime * CurrentTransitionType.TransitionSpeed;
				if (transitionAmount <= 0f)
				{
					InTransition = false;
					currentState = TransitionState.None;
					transitionAmount = 0f;
				}
			}
		}
		foreach (TransitionType transitionType in TransitionTypes)
		{
			transitionType.Material.SetFloat("_TransitionAmount", transitionAmount);
		}
	}

	private void OnApplicationQuit()
	{
		foreach (TransitionType transitionType in TransitionTypes)
		{
			transitionType.Material.SetFloat("_TransitionAmount", 0f);
		}
	}

	public void StartTransition(Action onTransition, float wantedHoldTime = 0.2f)
	{
		StartTransition(onTransition, TransitionTypes[0], wantedHoldTime);
	}

	public void StartTransition(Action onTransition, string id, float wantedHoldTime = 0.2f)
	{
		TransitionType transitionType = TransitionTypes.FirstOrDefault((TransitionType x) => x.Id == id);
		if (transitionType == null)
		{
			Debug.LogError("No transition found with id '" + id + "'");
		}
		else
		{
			StartTransition(onTransition, transitionType, wantedHoldTime);
		}
	}

	private void StartTransition(Action onTransition, TransitionType transitionType, float wantedHoldTime = 0.2f)
	{
		CurrentTransitionType = transitionType;
		TransitionImage.material = CurrentTransitionType.Material;
		currentState = TransitionState.Entering;
		InTransition = true;
		holdTimer = 0f;
		this.wantedHoldTime = wantedHoldTime;
		this.onTransition = onTransition;
	}
}
