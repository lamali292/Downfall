using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.Awakened.Token;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Powers.Awakened;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Downfall.Code.Cards.Awakened.Rare;

[Pool(typeof(AwakenedCardPool))]
public class FerventWorship : AwakenedCardModel
{
    public FerventWorship() : base(1, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        WithEnergyTip();
        WithTip(typeof(Ceremony));
        WithTip(StaticHoverTip.ReplayStatic);
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<FerventWorshipPower>(this, 1);
    }

    protected override void OnUpgrade()
    {
        // TODO
    }
}