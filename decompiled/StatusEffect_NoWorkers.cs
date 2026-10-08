using UnityEngine;

public class StatusEffect_NoWorkers : StatusEffect
{
	private float velo;

	protected override string TermId => "no_workers";

	public override Sprite Sprite => SpriteManager.instance.NoWorkersEffect;

	public override bool FadeInNonDefaultView => false;

	public override void Update()
	{
		FillAmount = FRILerp.Spring(FillAmount.HasValue ? FillAmount.Value : 0f, (float)base.ParentCard.MyGameCard.WorkerChildren.Count / (float)base.ParentCard.WorkerAmount, 10f, 10f, ref velo);
		base.Update();
	}
}
