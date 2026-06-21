using BaseLib.Utils;
using Collector.CollectorCode.Core;
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
        WithPower<CollectorDoomPower>(6, 2);
        WithDamage(6, 2);
        WithKeyword(CardKeyword.Exhaust);
        WithCards(1);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Apply<CollectorDoomPower>(ctx, this, cardPlay);
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await CollectorCmd.DrawCollected(ctx, Owner);
    }
}