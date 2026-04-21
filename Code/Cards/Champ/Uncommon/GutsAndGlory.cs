using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Powers.Champ;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Champ.Uncommon;

[Pool(typeof(ChampCardPool))]
public class GutsAndGlory : ChampCardModel
{
    public GutsAndGlory() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<GloryPower>(5, 3);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<GloryPower>(this);
    }
}