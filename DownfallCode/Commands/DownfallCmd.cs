using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace Downfall.DownfallCode.Commands;

/// <summary>
///     Generic card/creature/run predicates shared across characters. Combat side-effects
///     (enemy retaliation, power steal) live in <see cref="DownfallCombatCmd" />; pet
///     summoning lives in <see cref="PetSummonCmd" />.
/// </summary>
public class DownfallCmd
{
    /// <summary>
    ///     True if the card doesn't belong to the owner's character pool, i.e. it's a
    ///     Status, Colorless, or Curse card, or a card from another character.
    /// </summary>
    public static bool IsOffclass(CardModel card)
    {
        return card.VisualCardPool != card.Owner.Character.CardPool;
    }

    /// <summary>
    ///     True if this card play would hit at least one enemy, whether the card is single-target
    ///     (e.g. <c>AnyEnemy</c>, where <paramref name="target"/> is the resolved creature) or
    ///     untargeted AoE (e.g. <c>AllEnemies</c>/<c>RandomEnemy</c>, where <paramref name="target"/>
    ///     is null). Checking <c>target?.Side == CombatSide.Enemy</c> directly is a bug: it's always
    ///     false for AoE cards since they're played with a null target.
    /// </summary>
    public static bool TargetsEnemy(CardModel card, Creature? target)
    {
        return card.MyGetTargets(target).Any(c => c.Side == CombatSide.Enemy);
    }

    public static bool IsDebuffed(Creature? creature)
    {
        return creature?.Powers.Any(e => e.TypeForCurrentAmount == PowerType.Debuff) ?? false;
    }

    public static bool IsMultiplayer => (RunManager.Instance.State?.Players.Count ?? 1) > 1;
}
