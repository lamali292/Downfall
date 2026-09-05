using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Extensions;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class DarkwillowKindling : CollectorCardModel
{
    public DarkwillowKindling() : base(3, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeyword(CardKeyword.Retain, UpgradeType.Add);
        WithKeyword(CardKeyword.Exhaust);
        WithPower<ReserveNextTurnPower>(2);
        WithKindle(3);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    public override async Task AfterCardExhausted(PlayerChoiceContext ctx, CardModel card,
        bool causedByEthereal)
    {
        if (card != this) return;
        await CommonActions.ApplySelf<ReserveNextTurnPower>(ctx, this);
        await CollectorCmd.Kindle(ctx, this);
    }
}