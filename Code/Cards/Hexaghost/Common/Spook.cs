using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Hexaghost.Common;

[Pool(typeof(HexaghostCardPool))]
public class Spook : HexaghostCardModel
{
    public Spook() : base(0, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithPower<WeakPower>(1, 1);
        WithKeywords(CardKeyword.Exhaust, CardKeyword.Retain);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await MyCommonActions.Apply<WeakPower>(ctx, this, cardPlay);
    }
}