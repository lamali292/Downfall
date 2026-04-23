using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Commands;

public static class MyCommonActions
{
    public static async Task Apply<T>(CardModel card, CardPlay? cardPlay = null) where T : PowerModel
    {
        if (cardPlay?.Target is null && card.TargetType == TargetType.AnyEnemy)
        {
            await ApplyToRandomEnemy<T>(card);
            return;
        }
        switch (card)
        {
            case { TargetType: TargetType.AnyEnemy or TargetType.AnyAlly or TargetType.AnyPlayer }:
                if (cardPlay is null) break;
                await ApplyToEnemy<T>(card, cardPlay);
                break;
            case { TargetType: TargetType.AllEnemies, CombatState: not null }:
                await ApplyToAllEnemies<T>(card);
                break;
            case { TargetType: TargetType.RandomEnemy, CombatState: not null }:
                await ApplyToRandomEnemy<T>(card);
                break;
        }
    }
    
    public static async Task ApplyToAllEnemies<T>(CardModel card) where T : PowerModel
    {
        if (card.CombatState == null) return;
        await CommonActions.Apply<T>(card.CombatState.Enemies, card);
    }
    
    public static async Task ApplyToRandomEnemy<T>(CardModel card) where T : PowerModel
    {
        if (card.CombatState == null) return;
        await CommonActions.Apply<T>(card.CombatState.HittableEnemies.TakeRandom(1, card.CombatState.RunState.Rng.CombatTargets), card);
    }
    
    public static async Task ApplyToEnemy<T>(CardModel card, CardPlay cardPlay) where T : PowerModel
    {
        if (cardPlay.Target is null) return;
        await CommonActions.Apply<T>(cardPlay.Target, card);
    }
    
     /// <summary>
  /// Performs an attack using a card's DamageVar or CalculatedDamageVar on the card play's target.
  /// </summary>
  /// <param name="card"></param>
  /// <param name="play"></param>
  /// <param name="hitCount"></param>
  /// <param name="vfx"></param>
  /// <param name="sfx"></param>
  /// <param name="tmpSfx"></param>
  /// <returns></returns>
  public static AttackCommand CardAttack(
    CardModel card,
    CardPlay play,
    int hitCount = 1,
    string? vfx = null,
    string? sfx = null,
    string? tmpSfx = null)
  {
    return CommonActions.CardAttack(card, play.Target, hitCount, vfx, sfx, tmpSfx);
  }

  /// <summary>
  /// Performs an attack using a card's DamageVar or CalculatedDamageVar on a specified target.
  /// </summary>
  /// <param name="card"></param>
  /// <param name="target"></param>
  /// <param name="hitCount"></param>
  /// <param name="vfx"></param>
  /// <param name="sfx"></param>
  /// <param name="tmpSfx"></param>
  /// <returns></returns>
  /// <exception cref="T:System.Exception"></exception>
  public static AttackCommand CardAttack(
    CardModel card,
    Creature? target,
    int hitCount = 1,
    string? vfx = null,
    string? sfx = null,
    string? tmpSfx = null)
  {
    if (card.DynamicVars.ContainsKey("CalculatedDamage"))
      return CommonActions.CardAttack(card, target, card.DynamicVars.CalculatedDamage, hitCount, vfx, sfx, tmpSfx);
    return card.DynamicVars.ContainsKey("Damage") ? CommonActions.CardAttack(card, target, card.DynamicVars.Damage.BaseValue, hitCount, vfx, sfx, tmpSfx) : throw new Exception($"Card {card.Title} does not have a damage variable supported by CommonActions.CardAttack");
  }

  /// <summary>
  /// Performs an attacking using a specified amount of damage on a target.
  /// </summary>
  /// <param name="card"></param>
  /// <param name="target"></param>
  /// <param name="damage"></param>
  /// <param name="hitCount"></param>
  /// <param name="vfx"></param>
  /// <param name="sfx"></param>
  /// <param name="tmpSfx"></param>
  /// <returns></returns>
  /// <exception cref="T:System.Exception"></exception>
  public static AttackCommand CardAttack(
    CardModel card,
    Creature? target,
    decimal damage,
    int hitCount = 1,
    string? vfx = null,
    string? sfx = null,
    string? tmpSfx = null)
  {
    var attackCommand = DamageCmd.Attack(damage).WithHitCount(hitCount).FromCard(card);
    var combatState = card.CombatState;
    switch (card.TargetType)
    {
      case TargetType.AnyEnemy:
        if (target == null)
          return attackCommand;
        attackCommand.Targeting(target);
        break;
      case TargetType.AllEnemies:
        if (combatState == null)
          return attackCommand;
        attackCommand.TargetingAllOpponents(combatState);
        break;
      case TargetType.RandomEnemy:
        if (combatState == null)
          return attackCommand;
        attackCommand.TargetingRandomOpponents(combatState);
        break;
      case TargetType.None:
      case TargetType.Self:
      case TargetType.AnyPlayer:
      case TargetType.AnyAlly:
      case TargetType.AllAllies:
      case TargetType.TargetedNoCreature:
      case TargetType.Osty:
      default:
        throw new Exception($"Unsupported AttackCommand target type {card.TargetType} for card {card.Title}");
    }
    if (vfx != null || sfx != null || tmpSfx != null)
      attackCommand.WithHitFx(vfx, sfx, tmpSfx);
    return attackCommand;
  }

  /// <summary>
  /// Performs an attacking using aCalculatedDamageVar on a target.
  /// </summary>
  /// <param name="card"></param>
  /// <param name="target"></param>
  /// <param name="calculatedDamage"></param>
  /// <param name="hitCount"></param>
  /// <param name="vfx"></param>
  /// <param name="sfx"></param>
  /// <param name="tmpSfx"></param>
  /// <returns></returns>
  /// <exception cref="T:System.Exception"></exception>
  public static AttackCommand CardAttack(
    CardModel card,
    Creature? target,
    CalculatedDamageVar calculatedDamage,
    int hitCount = 1,
    string? vfx = null,
    string? sfx = null,
    string? tmpSfx = null)
  {
    var attackCommand = DamageCmd.Attack(calculatedDamage).WithHitCount(hitCount).FromCard(card);
    var combatState = card.CombatState;
    switch (card.TargetType)
    {
      case TargetType.AnyEnemy:
        if (target == null)
          return attackCommand;
        attackCommand.Targeting(target);
        break;
      case TargetType.AllEnemies:
        if (combatState == null)
          return attackCommand;
        attackCommand.TargetingAllOpponents(combatState);
        break;
      case TargetType.RandomEnemy:
        if (combatState == null)
          return attackCommand;
        attackCommand.TargetingRandomOpponents(combatState);
        break;
      case TargetType.None:
      case TargetType.Self:
      case TargetType.AnyPlayer:
      case TargetType.AnyAlly:
      case TargetType.AllAllies:
      case TargetType.TargetedNoCreature:
      case TargetType.Osty:
      default:
        throw new Exception($"Unsupported AttackCommand target type {card.TargetType} for card {card.Title}");
    }
    if (vfx != null || sfx != null || tmpSfx != null)
      attackCommand.WithHitFx(vfx, sfx, tmpSfx);
    return attackCommand;
  }
    
    
}