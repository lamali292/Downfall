using BaseLib.Utils;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Powers.Collector;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Downfall.Code.Cards.Collector.Token;

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

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await MyCommonActions.Apply<CollectorDoomPower>(this, cardPlay);
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await CollectorCmd.DrawCollected(ctx, Owner);
    }
}