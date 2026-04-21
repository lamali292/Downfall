using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Monsters;

namespace Downfall.Code.Cards.Collector.Token;

// Act 1

// Underdocks

// Monster
[Pool(typeof(CollectibleCardPool))]
public class CorpseSlugCard()
    : Collectible<CorpseSlug>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.1f);

[Pool(typeof(CollectibleCardPool))]
public class SeapunkCard() : Collectible<Seapunk>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class SludgeSpinnerCard()
    : Collectible<SludgeSpinner>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.1f);

[Pool(typeof(CollectibleCardPool))]
public class ToadpoleCard() : Collectible<Toadpole>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.1f);

[Pool(typeof(CollectibleCardPool))]
public class CalcifiedCultistCard()
    : Collectible<CalcifiedCultist>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class DampCultistCard()
    : Collectible<DampCultist>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class LivingFogCard() : Collectible<LivingFog>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class GasBombCard() : Collectible<GasBomb>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class FossilStalkerCard()
    : Collectible<FossilStalker>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class GremlinMercCard()
    : Collectible<GremlinMerc>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class FatGremlinCard()
    : Collectible<FatGremlin>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class SneakyGremlinCard()
    : Collectible<SneakyGremlin>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class HauntedShipCard()
    : Collectible<HauntedShip>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class PunchConstructCard()
    : Collectible<PunchConstruct>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class SewerClamCard() : Collectible<SewerClam>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class TwoTailedRatCard()
    : Collectible<TwoTailedRat>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

// Elite
[Pool(typeof(CollectibleCardPool))]
public class SkulkingColonyCard()
    : Collectible<SkulkingColony>(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class PhantasmalGardenerCard()
    : Collectible<PhantasmalGardener>(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class TerrorEelCard()
    : Collectible<TerrorEel>(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, 0.3f);

// Boss
[Pool(typeof(CollectibleCardPool))]
public class WaterfallGiantCard()
    : Collectible<WaterfallGiant>(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class SoulFyshCard() : Collectible<SoulFysh>(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class LagavulinMatriarchCard()
    : Collectible<LagavulinMatriarch>(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, 0.3f);

// Overgrowth

// Monster
[Pool(typeof(CollectibleCardPool))]
public class NibbitCard() : Collectible<Nibbit>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class LeafSlimeSCard()
    : Collectible<LeafSlimeS>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class LeafSlimeMCard()
    : Collectible<LeafSlimeM>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class TwigSlimeSCard()
    : Collectible<TwigSlimeS>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class TwigSlimeMCard()
    : Collectible<TwigSlimeM>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class ShrinkerBeetleCard()
    : Collectible<ShrinkerBeetle>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class FuzzyWurmCrawlerCard()
    : Collectible<FuzzyWurmCrawler>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class InkletCard() : Collectible<Inklet>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class FlyconidCard() : Collectible<Flyconid>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class FogmogCard() : Collectible<Fogmog>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class EyeWithTeethCard()
    : Collectible<EyeWithTeeth>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class MawlerCard() : Collectible<Mawler>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class SnappingJaxfruitCard()
    : Collectible<SnappingJaxfruit>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class SlitheringStranglerCard()
    : Collectible<SlitheringStrangler>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class VineShamblerCard()
    : Collectible<VineShambler>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class CubexConstructCard()
    : Collectible<CubexConstruct>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class AxeRubyRaiderCard()
    : Collectible<AxeRubyRaider>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class AssassinRubyRaiderCard()
    : Collectible<AssassinRubyRaider>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class BruteRubyRaiderCard()
    : Collectible<BruteRubyRaider>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class CrossbowRubyRaiderCard()
    : Collectible<CrossbowRubyRaider>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class TrackerRubyRaiderCard()
    : Collectible<TrackerRubyRaider>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

// Elite
[Pool(typeof(CollectibleCardPool))]
public class BygoneEffigyCard()
    : Collectible<BygoneEffigy>(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class ByrdonisCard() : Collectible<Byrdonis>(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class PhrogParasiteCard()
    : Collectible<PhrogParasite>(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class WrigglerCard() : Collectible<Wriggler>(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, 0.3f);

// Boss
[Pool(typeof(CollectibleCardPool))]
public class CeremonialBeastCard()
    : Collectible<CeremonialBeast>(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class KinPriestCard() : Collectible<KinPriest>(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class KinFollowerCard()
    : Collectible<KinFollower>(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class VantomCard() : Collectible<Vantom>(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, 0.3f);

// Act 2

// Hive

// Monster
[Pool(typeof(CollectibleCardPool))]
public class BowlbugRockCard()
    : Collectible<BowlbugRock>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class BowlbugEggCard()
    : Collectible<BowlbugEgg>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class BowlbugSilkCard()
    : Collectible<BowlbugSilk>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class BowlbugNectarCard()
    : Collectible<BowlbugNectar>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class ExoskeletonCard()
    : Collectible<Exoskeleton>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class ThievingHopperCard()
    : Collectible<ThievingHopper>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class TunnelerCard() : Collectible<Tunneler>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class ChomperCard() : Collectible<Chomper>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class HunterKillerCard()
    : Collectible<HunterKiller>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class LouseProgenitorCard()
    : Collectible<LouseProgenitor>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class MyteCard() : Collectible<Myte>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class OvicopterCard() : Collectible<Ovicopter>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class SlumberingBeetleCard()
    : Collectible<SlumberingBeetle>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class SpinyToadCard() : Collectible<SpinyToad>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class TheObscuraCard()
    : Collectible<TheObscura>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class ParafrightCard()
    : Collectible<Parafright>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

// Elite
[Pool(typeof(CollectibleCardPool))]
public class DecimillipedeBackCard()
    : Collectible<DecimillipedeSegmentBack>(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class DecimillipedeMiddleCard()
    : Collectible<DecimillipedeSegmentMiddle>(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class DecimillipedeFrontCard()
    : Collectible<DecimillipedeSegmentFront>(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class EntomancerCard()
    : Collectible<Entomancer>(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class InfestedPrismCard()
    : Collectible<InfestedPrism>(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, 0.3f);

// Boss
[Pool(typeof(CollectibleCardPool))]
public class TheInsatiableCard()
    : Collectible<TheInsatiable>(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class KnowledgeDemonCard()
    : Collectible<KnowledgeDemon>(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class KaiserCrusherCard() : Collectible<Crusher>(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class KaiserRocketCard() : Collectible<Rocket>(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, 0.3f);

// Act 3

// Glory

// Monster
[Pool(typeof(CollectibleCardPool))]
public class AxebotCard() : Collectible<Axebot>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class DevotedSculptorCard()
    : Collectible<DevotedSculptor>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class FabricatorCard()
    : Collectible<Fabricator>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class GuardbotCard() : Collectible<Guardbot>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class NoisebotCard() : Collectible<Noisebot>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class StabbotCard() : Collectible<Stabbot>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class ZapbotCard() : Collectible<Zapbot>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class FrogKnightCard()
    : Collectible<FrogKnight>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class GlobeHeadCard() : Collectible<GlobeHead>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class LivingShieldCard()
    : Collectible<LivingShield>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class TurretOperatorCard()
    : Collectible<TurretOperator>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class OwlMagistrateCard()
    : Collectible<OwlMagistrate>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class ScrollOfBitingCard()
    : Collectible<ScrollOfBiting>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class SlimedBerserkerCard()
    : Collectible<SlimedBerserker>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class TheLostCard() : Collectible<TheLost>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class TheForgottenCard()
    : Collectible<TheForgotten>(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 0.3f);

// Elite
[Pool(typeof(CollectibleCardPool))]
public class FlailKnightCard()
    : Collectible<FlailKnight>(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class SpectralKnightCard()
    : Collectible<SpectralKnight>(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class MagiKnightCard()
    : Collectible<MagiKnight>(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class MechaKnightCard()
    : Collectible<MechaKnight>(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class SoulNexusCard()
    : Collectible<SoulNexus>(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, 0.3f);

// Boss
[Pool(typeof(CollectibleCardPool))]
public class QueenCard() : Collectible<Queen>(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class TestSubjectCard()
    : Collectible<TestSubject>(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, 0.3f);

[Pool(typeof(CollectibleCardPool))]
public class DoormakerCard() : Collectible<Doormaker>(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, 0.3f);