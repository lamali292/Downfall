using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Powers.Downfall;
using Downfall.Code.Powers.Hexaghost;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Hexaghost.Rare;

[Pool(typeof(HexaghostCardPool))]
public class TurnItUp : HexaghostCardModel
{
    public TurnItUp() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithPower<TemporaryStrengthUpPower>(2, 1);
        WithPower<TemporaryDexterityUpPower>(2, 1);
        WithPower<TemporaryIntensityPower>(2, 1);
        WithKeyword(CardKeyword.Retain);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<TemporaryStrengthUpPower>(ctx, this);
        await CommonActions.ApplySelf<TemporaryDexterityUpPower>(ctx, this);
        await CommonActions.ApplySelf<TemporaryIntensityPower>(ctx, this);
    }
}