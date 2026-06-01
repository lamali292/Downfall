using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Extensions;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Champ.ChampCode.Cards.Common;

[Pool(typeof(ChampCardPool))]
public class BobAndWeave : ChampCardModel
{
    public BobAndWeave() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(4, 2);
        WithPower<VigorPower>(4, 1);
        this.WithEnterBerserker();
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.ApplySelf<VigorPower>(ctx, this);
    }
}