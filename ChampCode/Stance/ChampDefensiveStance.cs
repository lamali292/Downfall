using Champ.ChampCode.Core;
using Champ.ChampCode.CustomEnums;
using Champ.ChampCode.DynamicVars;
using Champ.ChampCode.Powers;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Champ.ChampCode.Stance;

public class ChampDefensiveStance : ChampStanceModel
{
    public override bool ShouldReceiveCombatHooks => true;
    public override bool HasFinisher => true;
    public override int MaxCharges => 2;
    public override string ChargeIconPathProgress => "res://Champ/images/ui/stance_defensive_progress.png";
    public override string ChargeIconPathOver => "res://Champ/images/ui/stance_defensive_over.png";
    public override string ChargeIconPathUnder => "res://Champ/images/ui/stance_defensive_under.png";
    public override Color? LabelOutlineColor => new("1745b0");


    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<CounterPower>(),
        HoverTipFactory.Static(StaticHoverTip.Block),
        HoverTipFactory.Static(ChampTip.Finisher)
    ];


    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DefensiveSkillVar(2),
        new DefensiveFinisherVar(6)
    ];

    public override async Task SkillBonus(PlayerChoiceContext ctx)
    {
        var amount = (int)((DefensiveSkillVar)DynamicVars["DefensiveSkill"]).Calculate();
        await PowerCmd.Apply<CounterPower>(ctx, Owner.Creature, amount, Owner.Creature, null);
    }

    public override async Task Finisher(PlayerChoiceContext ctx, bool affectsAllPlayers)
    {
        var amount = (int)((DefensiveFinisherVar)DynamicVars["DefensiveFinisher"]).Calculate();
        var targets = affectsAllPlayers
            ? Owner.AllTeammates.Select(e => e.Creature)
            : [Owner.Creature];
        await targets.ForEachAsync(e =>
             CreatureCmd.GainBlock(e, amount, BlockProps.nonCardUnpowered, null)
        );

    }
}