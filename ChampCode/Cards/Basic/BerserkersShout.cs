using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Extensions;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Champ.ChampCode.Cards.Basic;

[Pool(typeof(ChampCardPool))]
public class BerserkersShout : ChampCardModel
{
    public BerserkersShout() : base(0, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        WithPower<VigorPower>(2, 2);
        this.WithEnterBerserker();
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<VigorPower>(ctx, this);
    }
}