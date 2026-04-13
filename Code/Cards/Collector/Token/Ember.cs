using BaseLib.Utils;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Collector.Token;


[Pool(typeof(TokenCardPool))]
public class Ember : CollectorCardModel
{
    public Ember() : base(1, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
    {
        WithKeywords(CardKeyword.Retain, CardKeyword.Exhaust);
        WithPower<StrengthPower>(1, 1);
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if (card != this) return;
        await CommonActions.ApplySelf<StrengthPower>(this);
    }
}