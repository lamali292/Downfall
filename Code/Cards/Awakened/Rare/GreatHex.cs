using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Extensions;
using Downfall.Code.Interfaces;
using Downfall.Code.Powers.Awakened;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Awakened.Rare;

[Pool(typeof(AwakenedCardPool))]
public class GreatHex : AwakenedCardModel, IChantable
{
    public GreatHex() : base(1, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithKeywords(CardKeyword.Exhaust);
        WithPower<ManaburnPower>(5, 3);
    }

    public async Task PlayChantEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await CommonActions.Apply<GreatHexPower>(cardPlay.Target, this, DynamicVars.Power<ManaburnPower>().BaseValue);
    }
}