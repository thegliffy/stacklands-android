using UnityEngine;

public class DissolvingResource : Resource
{
	public float DissolvingTimeMultiplier = 1f;

	public AudioClip DissolveSound;

	public override void UpdateCard()
	{
		if (!HasStatusEffectOfType<StatusEffect_Dissolving>() && !MyGameCard.TimerRunningInStack)
		{
			AddStatusEffect(new StatusEffect_Dissolving());
		}
		else if (HasStatusEffectOfType<StatusEffect_Dissolving>() && MyGameCard.TimerRunningInStack)
		{
			RemoveStatusEffect<StatusEffect_Dissolving>();
		}
		base.UpdateCard();
	}

	public void Dissolve()
	{
		AudioManager.me.PlaySound(DissolveSound, base.transform, 1f, 0.5f);
		MyGameCard.DestroyCard(spawnSmoke: true);
	}
}
