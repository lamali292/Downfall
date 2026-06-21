using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Extensions;
using Champ.ChampCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Cards.Uncommon;

[Pool(typeof(ChampCardPool))]
public class
    WinnersCircle : ChampCardModel
{
    public WinnersCircle() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(7, 3);
        this.WithGlory(2, 1);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.ApplySelf<GloryPower>(ctx, this);
    }
}