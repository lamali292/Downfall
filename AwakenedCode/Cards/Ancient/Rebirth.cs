using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.CustomEnums;
using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Awakened.AwakenedCode.Cards.Ancient;

[Pool(typeof(AwakenedCardPool))]
public class Rebirth : AwakenedCardModel
{
    public Rebirth() : base(1, CardType.Power, CardRarity.Ancient, TargetType.Self)
    {
        this.WithPower<AwakeningPower>(8, 3, false);
        this.WithTip<VulnerablePower>();
        this.WithTip<WeakPower>();
        this.WithTip<FrailPower>();
        WithTip(AwakenedTip.Awaken);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<AwakeningPower>(ctx, this);
    }
}