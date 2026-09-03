using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Cards.Rare;

[Pool(typeof(CollectorCardPool))]
public class Omen : CollectorCardModel
{
    public Omen() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithPower<OmenPower>(1, false);
        WithKeyword(CardKeyword.Innate, UpgradeType.Add);
        WithTip<StrengthPower>();
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<OmenPower>(ctx, this);
    }
}