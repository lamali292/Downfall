using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Keywords;
using Downfall.Code.Powers.Awakened;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Awakened.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class RisingChorus : AwakenedCardModel
{
    public RisingChorus() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithKeyword(CardKeyword.Ethereal, UpgradeType.Remove);
        WithTip(DownfallTip.Chant);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<RisingChorusPower>(ctx, this, 1);
    }
}