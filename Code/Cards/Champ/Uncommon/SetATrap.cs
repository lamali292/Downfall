using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Champ.Uncommon;

[Pool(typeof(ChampCardPool))]
public class SetATrap : ChampCardModel
{
    public SetATrap() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithBlock(8, 2);
        WithPower<WeakPower>(2, 1);
    }

    protected override bool ShouldGlowGoldInternal => Owner.ShouldDefensiveComboTrigger();

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        await CommonActions.CardBlock(this, cardPlay);
        if (!Owner.ShouldDefensiveComboTrigger()) return;
        await CommonActions.Apply<WeakPower>(ctx,CombatState.HittableEnemies, this);
    }
}