using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Champ.ChampCode.Cards.Multiplayer;

[Pool(typeof(ChampCardPool))]
public class CrowdFavorite : ChampCardModel
{
    public CrowdFavorite() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
       this.WithPower<CrowdFavoritePower>(1, 1, false);
       this.WithTip<VigorPower>();
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;


    protected override Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return CommonActions.ApplySelf<CrowdFavoritePower>(ctx, this);
    }
}