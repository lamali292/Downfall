using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Cards.Rare;

[Pool(typeof(CollectorCardPool))]
public class DragonsTrove : CollectorCardModel
{
    public DragonsTrove() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeyword(CollectorKeyword.Pyre);
        WithReserve(1);
        WithKeyword(CardKeyword.Exhaust);
        WithCostUpgradeBy(-1);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (await PyreCmd.Pyre(ctx, this) == null) return;
        var cards = Owner.DrawPile.Where(e => e.VisualCardPool.IsColorless);
        await CardPileCmd.Add(cards, PileType.Hand);
        await ReserveCmd.GainReserve(this);
    }
}