using BaseLib.Utils;
using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace Collector.CollectorCode.Relics;

[Pool(typeof(CollectorRelicPool))]
public class EmeraldTorch : CollectorRelicModel
{
    public EmeraldTorch() : base(RelicRarity.Starter)
    {
        WithKindle(3);
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
        if (Owner != player) return false;
        if (!CollectorRewardsCmd.TryAddCollectiblesReward(this, player, cardRewardOptions, creationOptions)) return false;
        Flash();
        return true;
    }

}