using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace Awakened.AwakenedCode.Cards.Rare;

[Pool(typeof(AwakenedCardPool))]
public class EclipseEmbrace : AwakenedCardModel
{
    public EclipseEmbrace() : base(2, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        WithTip(CardKeyword.Exhaust);
        WithTip(typeof(Void));
        WithPower<EclipseEmbracePower>(1);
        WithEnergyTip();
        WithCostUpgradeBy(-1);
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<EclipseEmbracePower>(ctx, this);
    }
    
}