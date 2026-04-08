using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Awakened.Common;

[Pool(typeof(AwakenedCardPool))]
public class Psalm : AwakenedCardModel
{
    public Psalm() : base(2, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
    {
        WithDamage(10, 2);
        WithPower<WeakPower>(1, 1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await PowerCmd.Apply<WeakPower>(CombatState.Enemies, DynamicVars.Weak.BaseValue, Owner.Creature, this);
    }
}