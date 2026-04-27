using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Cards.Ancient;

[Pool(typeof(AwakenedCardPool))]
public class Rebirth : AwakenedCardModel
{
    public Rebirth() : base(1, CardType.Power, CardRarity.Ancient, TargetType.None)
    {
        WithPower<AwakeningPower>(8, 3);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<AwakeningPower>(ctx, this);
    }
}