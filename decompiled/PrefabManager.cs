using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrefabManager : MonoBehaviour
{
	public static PrefabManager instance;

	public Boosterpack BoosterpackPrefab;

	public GameCard GameCardPrefab;

	public HitText HitTextPrefab;

	public GameObject SmokeParticlePrefab;

	public GameObject ElectricityMinusParticlePrefab;

	public GameObject WellbeingPlusParticlePrefab;

	public GameObject SpiritBackgroundPlanePrefab;

	public GameObject FloatingTextPrefab;

	public SellBox SellBoxPrefab;

	public BuyBoosterBox BoosterBoxPrefab;

	public Statusbar StatusBarPrefab;

	public StatusEffectElement StatusEffectElementPrefab;

	public Projectile BulletProjectilePrefab;

	public Projectile RangedProjectilePrefab;

	public Projectile MagicProjectilePrefab;

	[Header("UI")]
	public CustomButton ButtonPrefab;

	public CustomButton CutsceneButtonPrefab;

	public CustomButton DebugButtonPrefab;

	public AchievementElement AchievementElementPrefab;

	public AchievementElement EmptyAchievementElementPrefab;

	public GameObject AchievementElementLabelPrefab;

	public NotificationElement NotificationElementPrefab;

	public RectTransform NormalLabelPrefab;

	public IdeaElement IdeaElementPrefab;

	public RebindElement RebindElementPrefab;

	public LayoutElement WellbeingLinePrefab;

	[Header("Hit Texts")]
	public HitText NormalHitText;

	public HitText MissHitText;

	public HitText BlockHitText;

	public HitText CritHitText;

	public HitText HealHitText;

	public HitText BleedHitText;

	public HitText PoisonHitText;

	public HitText SickHitText;

	public TextMeshPro IsVeryEffectiveText;

	private void Awake()
	{
		instance = this;
	}
}
