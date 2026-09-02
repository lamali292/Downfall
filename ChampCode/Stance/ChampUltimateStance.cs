using Champ.ChampCode.Core;
using Champ.ChampCode.CustomEnums;
using Champ.ChampCode.DynamicVars;
using Champ.ChampCode.Powers;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Champ.ChampCode.Stance;

public class ChampUltimateStance : ChampStanceModel
{
    public override bool ShouldReceiveCombatHooks => true;
    public override bool HasFinisher => true;
    public override string ChargeIconPathProgress => "res://Champ/images/ui/stance_ultimate_progress.png";
    public override string ChargeIconPathOver => "res://Champ/images/ui/stance_ultimate_over.png";
    public override string ChargeIconPathUnder => "res://Champ/images/ui/stance_ultimate_under.png";
    public override Color? LabelOutlineColor => new("5e3900");


    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<CounterPower>(),
        HoverTipFactory.FromPower<VigorPower>(),
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.Static(StaticHoverTip.Block),
        HoverTipFactory.Static(ChampTip.Finisher)
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BerserkerSkillVar(2),
        new DefensiveSkillVar(2),
        new BerserkerFinisherVar(1),
        new DefensiveFinisherVar(6)
    ];

    public override async Task SkillBonus(PlayerChoiceContext ctx)
    {
        var vigor = (int)((BerserkerSkillVar)DynamicVars["BerserkerSkill"]).Calculate();
        await PowerCmd.Apply<VigorPower>(ctx, Owner.Creature, vigor, Owner.Creature, null);

        var counter = (int)((DefensiveSkillVar)DynamicVars["DefensiveSkill"]).Calculate();
        await PowerCmd.Apply<CounterPower>(ctx, Owner.Creature, counter, Owner.Creature, null);
    }

    public override async Task Finisher(PlayerChoiceContext ctx, bool affectsAllPlayers)
    {
        var strength = (int)((BerserkerFinisherVar)DynamicVars["BerserkerFinisher"]).Calculate();
        var block = (int)((DefensiveFinisherVar)DynamicVars["DefensiveFinisher"]).Calculate();
        var targets = affectsAllPlayers
            ? Owner.AllTeammates.Select(e => e.Creature).ToList()
            : [Owner.Creature];
        await PowerCmd.Apply<StrengthPower>(ctx, targets, strength, Owner.Creature, null);
        await targets.ForEachAsync(e =>
            CreatureCmd.GainBlock(e, block, BlockProps.nonCardUnpowered, null)
        );
    }
}