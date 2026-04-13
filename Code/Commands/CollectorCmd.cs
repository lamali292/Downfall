using Godot;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.TestSupport;

namespace Downfall.Code.Commands;

public class CollectorCmd
{
  public static async Task<CardModel?> Pyre(PlayerChoiceContext ctx, CardModel card)
  {
    var prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1, 1);
    var pyred = (await CardSelectCmd.FromHand(ctx, card.Owner, prefs, e => e != card, card)).FirstOrDefault();
    if (pyred != null)
    {
      await CardCmd.Exhaust(ctx, pyred);
    }

    return pyred;
  }

  public static async Task<Creature> Summon<T>(
    PlayerChoiceContext ctx,
    Player summoner,
    int hp,
    AbstractModel? source) where T : MonsterModel
  {
    var combatState = summoner.Creature.CombatState;
    var existing = combatState.Allies.FirstOrDefault(c => c.Monster is T && c.PetOwner == summoner);
    
    var isReviving = existing != null && !existing.IsAlive;
    
    if (existing is { IsAlive: true })
    {
      await CreatureCmd.GainMaxHp(existing, hp);
      return existing;
    }
    
    if (isReviving)
      summoner.PlayerCombatState.AddPetInternal(existing!);
    else
    {
      existing = await PlayerCmd.AddPet<T>(summoner);
      var node = NCombatRoom.Instance?.GetCreatureNode(existing);
      if (node != null && source is CardModel)
      {
        node.Modulate = Colors.Transparent;
        node.CreateTween()
          .TweenProperty(node, "modulate", Colors.White, 0.35)
          .SetDelay(0.1);
      }
      await PowerCmd.Apply<DieForYouPower>(existing, 1M, null, null);
      node?.TrackBlockStatus(summoner.Creature);
    }

    await CreatureCmd.SetMaxHp(existing, hp);
    await CreatureCmd.Heal(existing, hp, isReviving);
    
    return existing!;
  }
  
}