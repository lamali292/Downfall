using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Interfaces;
using Downfall.DownfallCode.Abstract;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Cards.Rare;

[Pool(typeof(CollectorCardPool))]
public class DragonsTrove : CollectorCardModel, IHasPyre
{
    public DragonsTrove() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeyword(CollectorKeyword.Pyre);
        WithReserve(1);
        WithKeyword(CardKeyword.Exhaust);
        WithKeyword(CardKeyword.Retain, UpgradeType.Add);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    public CardModel? PyredCard { get; set; }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var cards = Owner.DrawPile.Where(e => e.VisualCardPool.IsColorless);
        await CardPileCmd.Add(cards, PileType.Hand);
        await CollectorCmd.GetReserve(this);
    }
}