using Champ.ChampCode.Core;
using Champ.ChampCode.DynamicVars;
using Champ.ChampCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Champ.ChampCode.Stance;

public class ChampDefensiveStance : ChampStanceModel
{
    public override bool ShouldReceiveCombatHooks => true;
    public override bool HasFinisher => true;
    public override string ChargeIconPath => "res://Champ/images/ui/stance_charge_defensive.png";

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
            ? Owner.GetAllPlayers().Select(e => e.Creature)
            : [Owner.Creature];
        await targets.ForEachAsync(e =>
             CreatureCmd.GainBlock(e, amount, BlockProps.nonCardUnpowered, null)
        );

    }
}