using BaseLib.Utils;
using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace Collector.CollectorCode.Relics;

[Pool(typeof(CollectorRelicPool))]
public class EmeraldTorch : CollectorRelicModel
{
    private bool active;
    public EmeraldTorch() : base(RelicRarity.Starter)
    {
        WithKindle(3);
        active = true;
    }
    
    public override RelicModel GetUpgradeReplacement()
    {
        return ModelDb.Relic<PrismaticTorch>();
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
    
    public override bool TryModifyCardRewardOptions(Player player, List<CardCreationResult> cardRewardOptions, CardCreationOptions creationOptions)
    {
        //reverted some code to fix white star bug.
        if (!active) return false;
        if (Owner != player) return false;
        var findFirst = CollectorRewardsCmd.TryAddCollectiblesReward(this, player, cardRewardOptions, creationOptions);
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