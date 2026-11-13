using Downfall.DownfallCode.Artists;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Powers;

namespace Snecko.SneckoCode.Cards.Multiplayer;

[Pool(typeof(SneckoCardPool))]
public class Venomology : SneckoCardModel
{
    protected override Artist Artist => Artist.Get<Fulgur>();
    protected override bool IsBetaArt => true;

    public Venomology() : base(3, CardType.Skill, CardRarity.Rare, TargetType.AnyAlly)
    {
        WithPower<VenomologyPower>(3, false);
        WithKeyword(CardKeyword.Exhaust, UpgradeType.Remove);
        WithTip<VenomPower>();
    }
    
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Apply<VenomologyPower>(ctx, this, cardPlay);
    }
}