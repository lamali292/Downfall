using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.Collector.Token;
using Downfall.Code.Commands;
using Downfall.Code.Core.Collector;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Downfall.Code.Relics.Collector;

[Pool(typeof(CollectorRelicPool))]
public class SoulLitLamp : CollectorRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<Ember>()];

    public override Task AfterObtained()
    {
        EssenceModel.AddEssence(Owner, 3);
        return Task.CompletedTask;
    }

    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext ctx,
        ICombatState combatState)
    {
        if (player != Owner) return;
        await DownfallCardCmd.GiveCard<Ember>(Owner, PileType.Hand);
    }
}