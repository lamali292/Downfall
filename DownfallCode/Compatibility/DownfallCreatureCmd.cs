using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.DownfallCode.Compatibility;

public static class DownfallCreatureCmd
{
    public static Task<IEnumerable<DamageResult>> Damage(
        PlayerChoiceContext choiceContext,
        Creature target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
#if V107
        return CreatureCmd.Damage(choiceContext, target, amount, props, dealer, cardSource);
#else
        return CreatureCmd.Damage(choiceContext, target, amount, props, dealer, cardSource, cardPlay);
#endif
    }

    public static Task<IEnumerable<DamageResult>> Damage(
        PlayerChoiceContext choiceContext,
        Creature target,
        decimal amount,
        ValueProp props,
        CardModel cardSource,
        CardPlay? cardPlay)
    {
        
#if V107
         return CreatureCmd.Damage(choiceContext, target, amount, props, cardSource);
#else
        return CreatureCmd.Damage(choiceContext, target, amount, props, cardSource, cardPlay);
#endif
    }

    public static Task<IEnumerable<DamageResult>> Damage(
        PlayerChoiceContext choiceContext,
        IEnumerable<Creature> targets,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
#if V107
        return CreatureCmd.Damage(choiceContext, targets, amount, props, dealer, cardSource);
#else
        return CreatureCmd.Damage(choiceContext, targets, amount, props, dealer, cardSource, cardPlay);
#endif
    }

}