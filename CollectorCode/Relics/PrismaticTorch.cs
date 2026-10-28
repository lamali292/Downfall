using BaseLib.Utils;
using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Runs;

namespace Collector.CollectorCode.Relics;

[Pool(typeof(CollectorRelicPool))]
public class PrismaticTorch : CollectorRelicModel
{
    
    public PrismaticTorch() : base(RelicRarity.Starter)
    {
        WithKindle(6);
    }

    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext ctx,
        ICombatState combatState)
    {
        if (player != Owner || Owner.PlayerCombatState is not { TurnNumber: 1 }) return;
        await TorchheadCmd.Kindle(ctx, this);
        Flash();
    }


    public override bool TryModifyCardRewardOptions(Player player, List<CardCreationResult> cardRewardOptions,
        CardCreationOptions creationOptions)
    {
        if (Owner != player) return false;
        if (!CollectorRewardsCmd.TryAddCollectiblesReward(this, player, cardRewardOptions, creationOptions, card => CardCmd.Upgrade(card))) return false;
        Flash();
        return true;
    }
    
}