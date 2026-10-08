using System;
using UnityEngine;

[Serializable]
public class GreedAnimationState
{
	[Term]
	public string TitleTerm;

	[Term]
	public string DescriptionTerm;

	[Card]
	public string CameraTargetId;

	[Term]
	public string ContinueTerm;

	public AudioClip Audio;
}
