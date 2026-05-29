using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using Downfall.DownfallCode.Artists;

namespace Collector.CollectorCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class BurningStrike : CollectorCardModel
{
    public BurningStrike() : base(1, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
    {
        WithTags(CardTag.Strike);
        WithDamage(14, 1);
        WithCards(1, 1);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await CommonActions.Draw(this, ctx);
    }
}