using Automaton.AutomatonCode.Cards.Basic;
using Automaton.AutomatonCode.Relics;
using Downfall.DownfallCode.Abstract;
using Downfall.DownfallCode.Config;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Core;

public class Automaton : DownfallCharacterModel
{
    private static readonly Color Color = new(0xD4C99DFF);
    public override Color EnergyLabelOutlineColor => new("4e3e01FF");
    public override string CharId => "Automaton";
    public override string ModId => AutomatonMainFile.ModId;
    public override Color NameColor => Color;
    public override Color LabOutlineColor => Color;
    public override Color DeckEntryCardColor => Color;
    public override float CardColorH => 0.16f;
    public override float CardColorS => 0.45f;
    public override float CardColorV => 1.2f;
    public override Color MapDrawingColor => new(0xFFFF00FF);

    public override bool HideFromVanillaCharacterSelect => DownfallConfig.HideAutomaton;
    public override bool HideInCompendium => DownfallConfig.HideAutomaton;
    
    public override CharacterGender Gender => CharacterGender.Neutral;
    protected override CharacterModel? UnlocksAfterRunAs => null;
    public override int StartingHp => 70;
    public override int StartingGold => 99;

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<BronzeCore>()
    ];

    public override float AttackAnimDelay => 0.15f;

    public override float CastAnimDelay => 0.25f;

    public override CardPoolModel CardPool => ModelDb.CardPool<AutomatonCardPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<AutomatonPotionPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<AutomatonRelicPool>();


    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<StrikeAutomaton>(),
        ModelDb.Card<StrikeAutomaton>(),
        ModelDb.Card<StrikeAutomaton>(),
        ModelDb.Card<StrikeAutomaton>(),
        ModelDb.Card<DefendAutomaton>(),
        ModelDb.Card<DefendAutomaton>(),
        ModelDb.Card<DefendAutomaton>(),
        ModelDb.Card<DefendAutomaton>(),
        ModelDb.Card<Postpone>(),
        ModelDb.Card<Branch>()
    ];
}

public class AutomatonCardPool : DownfallCardPool<Automaton>;

public class AutomatonPotionPool : DownfallPotionPool<Automaton>;

public class AutomatonRelicPool : DownfallRelicPool<Automaton>;

public abstract class AutomatonPowerModel(
    PowerType powerType = PowerType.Buff,
    PowerStackType powerStackType = PowerStackType.Counter) : DownfallPowerModel<Automaton>(powerType, powerStackType);

public abstract class AutomatonRelicModel(RelicRarity rarity, bool autoAdd = true)
    : DownfallRelicModel<Automaton>(rarity, autoAdd);

public abstract class AutomatonEnchantmentModel : DownfallEnchantmentModel<Automaton>;

public abstract class AutomatonPotionModel(PotionRarity potionRarity, PotionUsage potionUsage, TargetType targetType) :
    DownfallPotionModel<Automaton>(potionRarity, potionUsage, targetType);