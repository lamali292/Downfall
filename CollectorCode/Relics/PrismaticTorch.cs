using BaseLib.Utils;
using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace Collector.CollectorCode.Relics;

[Pool(typeof(CollectorRelicPool))]
public class PrismaticTorch : CollectorRelicModel
{
    
    private bool active;
    public PrismaticTorch() : base(RelicRarity.Starter)
    {
        WithKindle(7);
        active = true;
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
        if (!active) return false;
        if (Owner != player) return false;
        var findFirst = CollectorRewardsCmd.TryAddCollectiblesReward(this, player, cardRewardOptions, creationOptions, card => CardCmd.Upgrade(card));
        if (findFirst)
        {
            active = false;
            Flash();
        }
        else
        {
            return false;
        }
        return findFirst;
    }
    
    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        active = true;
    }
}