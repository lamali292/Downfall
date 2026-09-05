using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Relics;

[Pool(typeof(CollectorRelicPool))]
public class PrismaticTorch : CollectorRelicModel
{
    public PrismaticTorch() : base(RelicRarity.Starter)
    {
        WithKindle(10);
    }

    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext ctx,
        ICombatState combatState)
    {
        if (player != Owner || Owner.PlayerCombatState is not { TurnNumber: 1 }) return;
        await CollectorCmd.Kindle(ctx, this);
        Flash();
    }
    
}