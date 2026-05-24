using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Cards.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class MoonlitVision : AwakenedCardModel
{
    public MoonlitVision() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithEnergyTip();
        WithCostUpgradeBy(-1);
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<MoonlitVisionPower>(ctx, this, 1);
    }
}