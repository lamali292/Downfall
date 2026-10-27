using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Collector.CollectorCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class LuckyWick : CollectorCardModel
{
    public LuckyWick() : base(0, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
    {
        WithDamage(3, 2);
        WithBlock(2);
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override Artist Artist => Artist.Get<Thelethargicweirdo>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }
}