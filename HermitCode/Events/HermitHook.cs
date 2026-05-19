using Downfall.DownfallCode.Events;
using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Events;

public class HermitHook
{
    public static bool ShouldTriggerDeadOn(ICombatState cs, CardModel card)
        => DownfallHook.Any<IShouldTriggerDeadOn>(cs, e => e.ShouldTriggerDeadOn(card));

    public static Task AfterDeadOnTrigger(ICombatState cs,PlayerChoiceContext ctx, CardModel card, CardPlay cardPlay)
        => DownfallHook.Dispatch<IAfterDeadOnTrigger>(cs, e => e.AfterDeadOnTrigger(ctx, card, cardPlay));

    public static int ModifyDeadOnCount(ICombatState cs, int orignal, CardModel card, out IEnumerable<IModifyDeadOnCount> modifiers)
        => DownfallHook.Modify(cs, orignal, (e,amount) => e.ModifyDeadOnCount(amount, card), out modifiers);
    public static Task AfterModifyingDeadOnCount(ICombatState cs, PlayerChoiceContext ctx, CardModel card, IEnumerable<IModifyDeadOnCount> modifiers)
        => DownfallHook.AfterModifying(cs, modifiers, e => e.AfterModifyingDeadOnCount(ctx, card));
    
    public static bool ShouldPreventBruiseRemoval(ICombatState cs, BruisePower power, out IEnumerable<IShouldPreventBruiseRemoval> preventers)
        => DownfallHook.Any(cs, h => h.ShouldPreventBruiseRemoval(power), out preventers);
    public static Task AfterPreventedBruiseRemoval(ICombatState cs, BruisePower power, IEnumerable<IShouldPreventBruiseRemoval> preventers)
        => DownfallHook.AfterModifying(cs, preventers, h => h.AfterPreventedBruiseRemoval(power));
    
}





 