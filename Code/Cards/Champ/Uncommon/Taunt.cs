using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Champ.Uncommon;

[Pool(typeof(ChampCardPool))]
public class Taunt : ChampCardModel
{
    public Taunt() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithPower<VulnerablePower>(1);
        WithPower<WeakPower>(1);
    }


    public override TargetType TargetType => IsUpgraded ? TargetType.AllEnemies : TargetType.AnyEnemy;

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (IsUpgraded)
        {
            if (CombatState == null) return;
            var enemies = CombatState.HittableEnemies;
            await CommonActions.Apply<VulnerablePower>(ctx, enemies, this);
            await CommonActions.Apply<WeakPower>(ctx, enemies, this);
        }
        else
        {
            if (cardPlay.Target == null) return;
            await CommonActions.Apply<VulnerablePower>(ctx, cardPlay.Target, this);
            await CommonActions.Apply<WeakPower>(ctx, cardPlay.Target, this);
        }
    }
}