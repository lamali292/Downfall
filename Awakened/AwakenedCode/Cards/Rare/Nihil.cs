using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.CustomEnums;
using Awakened.AwakenedCode.Interfaces;
using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Awakened.AwakenedCode.Cards.Rare;

[Pool(typeof(AwakenedCardPool))]
public class Nihil : AwakenedCardModel, IChantable
{
    public Nihil() : base(2, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithPower<ManaburnPower>(13, 3);
        WithTip(AwakenedTip.Chant);
    }

    public async Task PlayChantEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState);
        foreach (var combatStateEnemy in CombatState.Enemies)
        {
            var a = combatStateEnemy.GetPowerAmount<ManaburnPower>();
            if (a <= 0) continue;
            await CreatureCmd.Damage(
                ctx,
                combatStateEnemy,
                a,
                ValueProp.Unpowered | ValueProp.Unblockable,
                this);
        }
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await CommonActions.Apply<ManaburnPower>(ctx, cardPlay.Target, this);
    }
}