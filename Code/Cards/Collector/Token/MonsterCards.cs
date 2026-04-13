using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Monsters;

namespace Downfall.Code.Cards.Collector.Token;


// Underdocks

// Monster
[Pool(typeof(CollectibleCardPool))]
public class CorpseSlugCard() : Collectible<CorpseSlug>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.1f);
[Pool(typeof(CollectibleCardPool))]
public class SeapunkCard() : Collectible<Seapunk>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);
[Pool(typeof(CollectibleCardPool))]
public class SludgeSpinnerCard() : Collectible<SludgeSpinner>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.1f);
[Pool(typeof(CollectibleCardPool))]
public class ToadpoleCard() : Collectible<Toadpole>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.1f);
[Pool(typeof(CollectibleCardPool))]
public class CalcifiedCultistCard() : Collectible<CalcifiedCultist>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);
[Pool(typeof(CollectibleCardPool))]
public class DampCultistCard() : Collectible<DampCultist>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);
[Pool(typeof(CollectibleCardPool))]
public class LivingFogCard() : Collectible<LivingFog>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);
[Pool(typeof(CollectibleCardPool))]
public class FossilStalkerCard() : Collectible<FossilStalker>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);
[Pool(typeof(CollectibleCardPool))]
public class FatGremlinCard() : Collectible<FatGremlin>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);
[Pool(typeof(CollectibleCardPool))]
public class SneakyGremlinCard() : Collectible<SneakyGremlin>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);
[Pool(typeof(CollectibleCardPool))]
public class HauntedShipCard() : Collectible<HauntedShip>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);
[Pool(typeof(CollectibleCardPool))]
public class PunchConstructCard() : Collectible<PunchConstruct>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);
[Pool(typeof(CollectibleCardPool))]
public class SewerClamCard() : Collectible<SewerClam>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);
[Pool(typeof(CollectibleCardPool))]
public class TwoTailedRatCard() : Collectible<TwoTailedRat>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

// Elite
[Pool(typeof(CollectibleCardPool))]
public class SkulkingColonyCard() : Collectible<SkulkingColony>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);
[Pool(typeof(CollectibleCardPool))]
public class PhantasmalGardenerCard() : Collectible<PhantasmalGardener>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);
[Pool(typeof(CollectibleCardPool))]
public class TerrorEelCard() : Collectible<TerrorEel>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

// Boss
[Pool(typeof(CollectibleCardPool))]
public class WaterfallGiantCard() : Collectible<WaterfallGiant>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);
[Pool(typeof(CollectibleCardPool))]
public class SoulFyshCard() : Collectible<SoulFysh>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);
[Pool(typeof(CollectibleCardPool))]
public class LagavulinMatriarchCard() : Collectible<LagavulinMatriarch>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);



// Overgrowth

// Monster
[Pool(typeof(CollectibleCardPool))]
public class NibbitCard() : Collectible<Nibbit>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);
[Pool(typeof(CollectibleCardPool))]
public class LeafSlimeSCard() : Collectible<LeafSlimeS>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);
[Pool(typeof(CollectibleCardPool))]
public class LeafSlimeMCard() : Collectible<LeafSlimeM>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);
[Pool(typeof(CollectibleCardPool))]
public class TwigSlimeSCard() : Collectible<TwigSlimeS>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);
[Pool(typeof(CollectibleCardPool))]
public class TwigSlimeMCard() : Collectible<TwigSlimeM>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);
[Pool(typeof(CollectibleCardPool))]
public class ShrinkerBeetleCard() : Collectible<ShrinkerBeetle>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);
[Pool(typeof(CollectibleCardPool))]
public class FuzzyWurmCrawlerCard() : Collectible<FuzzyWurmCrawler>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);
[Pool(typeof(CollectibleCardPool))]
public class InkletCard() : Collectible<Inklet>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);
[Pool(typeof(CollectibleCardPool))]
public class FlyconidCard() : Collectible<Flyconid>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);
[Pool(typeof(CollectibleCardPool))]
public class FogmogCard() : Collectible<Fogmog>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);
[Pool(typeof(CollectibleCardPool))]
public class EyeWithTeethCard() : Collectible<EyeWithTeeth>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);
[Pool(typeof(CollectibleCardPool))]
public class MawlerCard() : Collectible<Mawler>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);
[Pool(typeof(CollectibleCardPool))]
public class SnappingJaxfruitCard() : Collectible<SnappingJaxfruit>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);
[Pool(typeof(CollectibleCardPool))]
public class SlitheringStranglerCard() : Collectible<SlitheringStrangler>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);
[Pool(typeof(CollectibleCardPool))]
public class VineShamblerCard() : Collectible<VineShambler>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);
[Pool(typeof(CollectibleCardPool))]
public class CubexConstructCard() : Collectible<CubexConstruct>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);
[Pool(typeof(CollectibleCardPool))]
public class AxeRubyRaiderCard() : Collectible<AxeRubyRaider>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);
[Pool(typeof(CollectibleCardPool))]
public class AssassinRubyRaiderCard() : Collectible<AssassinRubyRaider>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);
[Pool(typeof(CollectibleCardPool))]
public class BruteRubyRaiderCard() : Collectible<BruteRubyRaider>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);
[Pool(typeof(CollectibleCardPool))]
public class CrossbowRubyRaiderCard() : Collectible<CrossbowRubyRaider>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);
[Pool(typeof(CollectibleCardPool))]
public class TrackerRubyRaiderCard() : Collectible<TrackerRubyRaider>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);


// Elite
[Pool(typeof(CollectibleCardPool))]
public class BygoneEffigyCard() : Collectible<BygoneEffigy>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);
[Pool(typeof(CollectibleCardPool))]
public class ByrdonisCard() : Collectible<Byrdonis>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);
[Pool(typeof(CollectibleCardPool))]
public class PhrogParasiteCard() : Collectible<PhrogParasite>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

// Boss
[Pool(typeof(CollectibleCardPool))]
public class CeremonialBeastCard() : Collectible<CeremonialBeast>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);
[Pool(typeof(CollectibleCardPool))]
public class KinPriestCard() : Collectible<KinPriest>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);
[Pool(typeof(CollectibleCardPool))]
public class VantomCard() : Collectible<Vantom>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);