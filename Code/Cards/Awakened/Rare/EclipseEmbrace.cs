using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Powers.Awakened;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace Downfall.Code.Cards.Awakened.Rare;

[Pool(typeof(AwakenedCardPool))]
public class EclipseEmbrace : AwakenedCardModel
{
    public EclipseEmbrace() : base(2, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        WithTip(CardKeyword.Exhaust);
        WithTip(typeof(Void));
        WithPower<EclipseEmbracePower>(1);
        WithEnergyTip();
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await MyCommonActions.ApplySelf<EclipseEmbracePower>(this);
    }


    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}