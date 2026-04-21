using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Powers.Champ;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Champ.Rare;

[Pool(typeof(ChampCardPool))]
public class UltimateStance : ChampCardModel
{
    public UltimateStance() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithPower<GloryPower>(10);
        WithCostUpgradeBy(-1);
        WithKeywords(CardKeyword.Retain, CardKeyword.Exhaust);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<GloryPower>(this);
    }
}