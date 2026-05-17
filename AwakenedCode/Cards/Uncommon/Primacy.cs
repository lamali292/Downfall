using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Awakened.AwakenedCode.Cards.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class Primacy : AwakenedCardModel
{
    public Primacy() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithPower<PrimacyPower>(1, 1, false);
        WithTip(typeof(StrengthPower));
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<PrimacyPower>(ctx, this);
    }
}