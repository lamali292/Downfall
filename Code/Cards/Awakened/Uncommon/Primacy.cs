using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Extensions;
using Downfall.Code.Powers.Awakened;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Awakened.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class Primacy : AwakenedCardModel
{
    public Primacy() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithPower<PrimacyPower>(1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<PrimacyPower>(this, DynamicVars.Power<PrimacyPower>().BaseValue);
    }


    protected override void OnUpgrade()
    {
        DynamicVars.Power<PrimacyPower>().UpgradeValueBy(1);
    }
}