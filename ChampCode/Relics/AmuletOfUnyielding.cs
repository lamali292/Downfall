using BaseLib.Extensions;
using BaseLib.Utils;
using Champ.ChampCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace Champ.ChampCode.Relics;

[Pool(typeof(ChampRelicPool))]
public class AmuletOfUnyielding : ChampRelicModel
{
    [SavedProperty]
    // ReSharper disable once MemberCanBePrivate.Global
    public int VigorProgress { get; private set; }

    public AmuletOfUnyielding() : base(RelicRarity.Rare)
    {
        WithPower<StrengthPower>(1);
        WithPower<VigorPower>(12);
    }

    public override bool ShowCounter => true;

    public override int DisplayAmount => IsActivating ? VigorThreshold : VigorProgress;

    private bool IsActivating
    {
        get;
        set
        {
            AssertMutable();
            field = value;
            InvokeDisplayAmountChanged();
        }
    }

    private int VigorThreshold => DynamicVars.Power<VigorPower>().IntValue;
    private int StrengthMult => DynamicVars.Power<StrengthPower>().IntValue;

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext ctx, PowerModel power, decimal amount,
        Creature? applier, CardModel? cardSource)
    {
        if (power.Owner != Owner.Creature || power is not VigorPower || amount >= 0) return;

        VigorProgress += (int)-amount;

        var thresholdsCrossed = VigorProgress / VigorThreshold;
        if (thresholdsCrossed > 0)
        {
            VigorProgress -= thresholdsCrossed * VigorThreshold;
            _ = TaskHelper.RunSafely(DoActivateVisuals());
            await PowerCmd.Apply<StrengthPower>(ctx, Owner.Creature, thresholdsCrossed * StrengthMult, Owner.Creature, null);
        }
        InvokeDisplayAmountChanged();
    }

    private async Task DoActivateVisuals()
    {
        IsActivating = true;
        Flash();
        await Cmd.Wait(1f);
        IsActivating = false;
    }
}