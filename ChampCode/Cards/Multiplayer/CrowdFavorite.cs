using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Powers;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Champ.ChampCode.Cards.Multiplayer;

[Pool(typeof(ChampCardPool))]
public class CrowdFavorite : ChampCardModel
{
    public CrowdFavorite() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithPower<CrowdFavoritePower>(1, 1, false);
        WithTip<VigorPower>();
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    protected override Artist Artist => Artist.Get<Fulgur>();
    protected override bool IsBetaArt => true;


    protected override Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return CommonActions.ApplySelf<CrowdFavoritePower>(ctx, this);
    }
}