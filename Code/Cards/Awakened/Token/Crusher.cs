using BaseLib.Utils;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Downfall.Code.Cards.Awakened.Token;

[Pool(typeof(TokenCardPool))]
public class Crusher : AwakenedCardModel
{
    public Crusher() : base(5, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
    {
        WithDamage(25, 5);
        WithKeywords(CardKeyword.Retain);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }

    public override Task AfterCardGeneratedForCombat(CardModel card, bool addedByPlayer)
    {
        if (card.Owner != Owner) return Task.CompletedTask;
        EnergyCost.AddUntilPlayed(-1);
        return Task.CompletedTask;
    }
}