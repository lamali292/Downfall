using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Events;
using Collector.CollectorCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Collector.CollectorCode.Relics;

[Pool(typeof(CollectorRelicPool))]
public class BlockedChakra : CollectorRelicModel, IModifyTorchheadRekindle
{
    public BlockedChakra() : base(RelicRarity.Shop)
    {
        WithKindle(5);
        WithTip(CollectorKeyword.Torchhead);
    }

    public int ModifyTorchheadRekindle(Player player, int current)
    {
        return player == Owner ? current + DynamicVars.Kindle.IntValue : current;
    }
}