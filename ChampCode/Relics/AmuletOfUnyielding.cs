using BaseLib.Extensions;
using BaseLib.Utils;
using Champ.ChampCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Champ.ChampCode.Relics;

[Pool(typeof(ChampRelicPool))]
public class AmuletOfUnyielding : ChampRelicModel
{
    private int _strengthGranted;
    private decimal _vigorSpentThisCombat;

    public AmuletOfUnyielding() : base(RelicRarity.Rare)
    {
        WithPower<StrengthPower>(1);
        WithPower<VigorPower>(12);
    }

    public override bool ShowCounter => CombatManager.Instance.IsInProgress;

    public override int DisplayAmount => (int)(_vigorSpentThisCombat % VigorThreshold);
    private int VigorThreshold => DynamicVars.Power<VigorPower>().IntValue;
    private int StrengthMult => DynamicVars.Power<StrengthPower>().IntValue;

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext ctx, PowerModel power, decimal amount,
        Creature? applier, CardModel? cardSource)
    {
        if (power.Owner != Owner.Creature || power is not VigorPower || amount >= 0) return;

        _vigorSpentThisCombat -= amount;

        InvokeDisplayAmountChanged();
        var totalEarned = (int)(_vigorSpentThisCombat / VigorThreshold);
        var toGain = totalEarned - _strengthGranted;
        if (toGain <= 0) return;

        _strengthGranted = totalEarned;
        toGain *= StrengthMult;
        await PowerCmd.Apply<StrengthPower>(ctx, Owner.Creature, toGain, Owner.Creature, null);
        Flash();
    }
}