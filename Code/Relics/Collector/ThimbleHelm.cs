using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Commands;
using Downfall.Code.Keywords;
using Downfall.Code.Powers.Collector;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Downfall.Code.Relics.Collector;

[Pool(typeof(CollectorRelicPool))]
public class ThimbleHelm : CollectorRelicModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [DownfallTip.Kindle.ToHoverTip()];


    public override RelicRarity Rarity => RelicRarity.Rare;

    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        CombatState combatState)
    {
        if (player != Owner) return;
        await CollectorCmd.SummonTorchhead(choiceContext, Owner, 3, this);
        await PowerCmd.Apply<ThimbleHelmPower>(Owner.Creature, 1, Owner.Creature, null);
    }
}