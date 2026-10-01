using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class Equalize : SlimeBossCardModel
{
    public Equalize() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(12, 4);
        WithTip(SlimeBossTip.Consume);
        WithTip(StaticHoverTip.Block);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var result = await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        var damage = result.Results.SelectMany(e => e).Sum(e => e.BlockedDamage + e.UnblockedDamage);
        // TODO scale with dex?
        await SlimeBossCmd.Consume(ctx, this, cardPlay, _ => damage > 0
            ? CreatureCmd.GainBlock(Owner.Creature, damage, BlockProps.cardUnpowered, cardPlay)
            : Task.CompletedTask);
    }
}
