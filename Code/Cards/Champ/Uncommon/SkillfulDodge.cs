using BaseLib.Extensions;
using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Extensions;
using Downfall.Code.Powers.Champ;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Champ.Uncommon;

[Pool(typeof(ChampCardPool))]
public class SkillfulDodge : ChampCardModel
{
    public SkillfulDodge() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(4, 1);
        WithPower<CounterPower>(4, 1);
        WithVar("Increase", 3, 1);
    }

    protected override bool ShouldGlowGoldInternal => Owner.ShouldDefensiveComboTrigger();

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.ApplySelf<CounterPower>(this);
        if (!Owner.ShouldDefensiveComboTrigger()) return;
        DynamicVars.Block.UpgradeValueBy(DynamicVars["Increase"].IntValue);
        DynamicVars.Power<CounterPower>().UpgradeValueBy(DynamicVars["Increase"].IntValue);
    }
}