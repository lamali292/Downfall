using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Cards.Token;


[Pool(typeof(TokenCardPool))]
public class Mushroom : CollectorCardModel
{
    public Mushroom() : base(0, CardType.Skill, CardRarity.Token, TargetType.AnyEnemy)
    {
        WithBlock(8, 2);
        WithPower<VulnerablePower>(2,1);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.Apply<VulnerablePower>(ctx, this, cardPlay);
    }
}