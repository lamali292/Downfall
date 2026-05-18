using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Cards.Uncommon;

[Pool(typeof(ChampCardPool))]
public class GutsAndGlory : ChampCardModel
{
    public GutsAndGlory() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithGlory(5, 3);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<GloryPower>(ctx, this);
    }
}