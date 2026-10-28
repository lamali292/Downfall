using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Extensions;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Collector.CollectorCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class Blightning : CollectorCardModel
{
    public Blightning() : base(0, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
    {
        WithTorchheadDamage(9, 2);
        WithPower<BlisterPower>(3, 1);
        WithCards(2);
        WithKeyword(CardKeyword.Exhaust);
        WithTags(CardTag.Strike);
        WithTip(CollectorKeyword.Torchhead);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        await TorchheadCmd.Attack(ctx, this, cardPlay);
        await CommonActions.Apply<BlisterPower>(ctx, this, cardPlay);
        await CommonActions.Draw(this, ctx);

    }
}