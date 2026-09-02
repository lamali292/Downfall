using BaseLib.Utils;
using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Relics;

[Pool(typeof(CollectorRelicPool))]
public class EmeraldTorch() : CollectorRelicModel(RelicRarity.Starter)
{
    public override RelicModel GetUpgradeReplacement()
    {
        WithVar("KindleAmount", 4);
        return ModelDb.Relic<PrismaticTorch>();
    }
    private DynamicVar KindleAmount => DynamicVars["KindleAmount"];
    
    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext ctx,
        ICombatState combatState)
    {
        if (player != Owner || Owner.PlayerCombatState is not { TurnNumber: 1 }) return;
        var dV = (int)KindleAmount.BaseValue;
        await CollectorCmd.SummonTorchhead(ctx, Owner, dV, this);
        Flash();
    }
}