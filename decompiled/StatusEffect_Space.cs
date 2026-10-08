using UnityEngine;

public class StatusEffect_Space : StatusEffect
{
	protected override string TermId => "space";

	public Apartment apartment => base.ParentCard as Apartment;

	public override Sprite Sprite => SpriteManager.instance.HousingSpaceEffect;

	public override string Description => SokLoc.Translate("statuseffect_space_description", LocParam.Create("amount", apartment.FreeSpace.ToString()));

	public override int? StatusNumber => apartment.FreeSpace;

	public override Color? StatusNumberColor => Color.black;
}
