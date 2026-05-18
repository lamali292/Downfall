using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Cards.Common;

[Pool(typeof(ChampCardPool))]
public class VictoryIsMine : ChampCardModel
{
    public VictoryIsMine() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(3, 1);
        WithGlory(2, 1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.ApplySelf<GloryPower>(ctx, this);
    }
}