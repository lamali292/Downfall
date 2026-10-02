using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.CustomEnums;
using Awakened.AwakenedCode.Powers;
using BaseLib.Abstracts;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Cards.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class RisingChorus : AwakenedCardModel
{
    public RisingChorus() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeyword(CardKeyword.Ethereal, UpgradeType.Remove);
        WithTip(AwakenedKeyword.Chant);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<RisingChorusPower>(ctx, this, 1);
    }
}