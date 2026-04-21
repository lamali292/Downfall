using BaseLib.Utils;
using Downfall.Code.Abstract.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Downfall.Code.Cards.Collector.Token;

[Pool(typeof(TokenCardPool))]
public class LuckyWick : CollectorCardModel
{
    public LuckyWick() : base(0, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
    {
        WithDamage(3, 1);
        WithBlock(3, 1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }
}