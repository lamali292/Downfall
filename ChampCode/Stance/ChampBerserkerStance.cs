using Champ.ChampCode.Core;
using Champ.ChampCode.DynamicVars;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Champ.ChampCode.Stance;

public class ChampBerserkerStance : ChampStanceModel
{
    public override bool ShouldReceiveCombatHooks => true;

    public override int MaxCharges => 2;
    public override bool HasFinisher => true;
    public override string ChargeIconPathProgress => "res://Champ/images/ui/stance_berserker_progress.png";
    public override string ChargeIconPathOver => "res://Champ/images/ui/stance_berserker_over.png";
    public override string ChargeIconPathUnder => "res://Champ/images/ui/stance_berserker_under.png";
    public override Color? LabelOutlineColor => new("700000");
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BerserkerSkillVar(2),
        new BerserkerFinisherVar(1)
    ];

    public override async Task SkillBonus(PlayerChoiceContext ctx)
    {
        var amount = (int)((BerserkerSkillVar)DynamicVars["BerserkerSkill"]).Calculate();
        await PowerCmd.Apply<VigorPower>(ctx, Owner.Creature, amount, Owner.Creature, null);
    }

    public override async Task Finisher(PlayerChoiceContext ctx, bool affectsAllPlayers)
    {
        var amount = (int)((BerserkerFinisherVar)DynamicVars["BerserkerFinisher"]).Calculate();
        var targets = affectsAllPlayers
            ? Owner.AllTeammates.Select(e => e.Creature)
            : [Owner.Creature];
        await PowerCmd.Apply<StrengthPower>(ctx, targets, amount, Owner.Creature, null);
    }
}