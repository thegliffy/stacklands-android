using UnityEngine;

public class StatusEffect_NoEnergy : StatusEffect
{
	protected override string TermId => "no_energy";

	public override string Description => SokLoc.Translate("statuseffect_no_energy_description");

	public override bool FadeInNonDefaultView => false;

	public override Sprite Sprite => SpriteManager.instance.NoEnergyEffect;
}
