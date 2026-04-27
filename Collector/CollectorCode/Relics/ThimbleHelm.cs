using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Collector.CollectorCode.Relics;

[Pool(typeof(CollectorRelicPool))]
public class ThimbleHelm : CollectorRelicModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [CollectorTip.Kindle.ToHoverTip()];


    public override RelicRarity Rarity => RelicRarity.Rare;

    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext ctx,
        ICombatState combatState)
    {
        if (player != Owner) return;
        await CollectorCmd.SummonTorchhead(ctx, Owner, 3, this);
        await PowerCmd.Apply<ThimbleHelmPower>(ctx, Owner.Creature, 1, Owner.Creature, null);
    }
}