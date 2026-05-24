using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.CustomEnums;
using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Awakened.AwakenedCode.Cards.Ancient;

[Pool(typeof(AwakenedCardPool))]
public class Rebirth : AwakenedCardModel
{
    public Rebirth() : base(1, CardType.Power, CardRarity.Ancient, TargetType.Self)
    {
        WithPower<AwakeningPower>(8, 3, false);
        WithTip(typeof(VulnerablePower));
        WithTip(typeof(WeakPower));
        WithTip(typeof(FrailPower));
        WithTip(AwakenedTip.Awaken);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<AwakeningPower>(ctx, this);
    }
}