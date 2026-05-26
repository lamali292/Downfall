using System.Globalization;
using Champ.ChampCode.Core;
using Champ.ChampCode.DynamicVars;
using Champ.ChampCode.Events;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Champ.ChampCode.Stance;

public class ChampBerserkerStance : ChampStanceModel
{
    public override bool ShouldReceiveCombatHooks => true;

    public override bool HasFinisher => true;
    public override string ChargeIconPath => "res://Champ/images/ui/stance_charge_berserker.png";

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

    public override async Task Finisher(PlayerChoiceContext ctx)
    {
        var amount = (int)((BerserkerFinisherVar)DynamicVars["BerserkerFinisher"]).Calculate();
        await PowerCmd.Apply<StrengthPower>(ctx, Owner.Creature, amount, Owner.Creature, null);
    }

   

}