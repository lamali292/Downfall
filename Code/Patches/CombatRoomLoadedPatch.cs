using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Cards.Vfx;
using Downfall.Code.Character;
using Downfall.Code.Commands;
using Downfall.Code.Powers.Awakened;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using NSequenceDisplay = Downfall.Code.Cards.Vfx.NSequenceDisplay;

namespace Downfall.Code.Patches;

[HarmonyPatch(typeof(CombatManager), nameof(CombatManager.AfterCombatRoomLoaded))]
public static class CombatRoomLoadedPatch
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        var combatState = CombatManager.Instance.DebugOnlyGetState();
        if (combatState == null) return;

        foreach (var player in combatState.Players)
        {
            if (!LocalContext.IsMe(player)) continue;

            var creature = player.Creature;
            var combatRoom = NCombatRoom.Instance;
            if (combatRoom == null) continue;

            var creatureNode = combatRoom.GetCreatureNode(creature);
            var vfxContainer = combatRoom.CombatVfxContainer;
            
            switch (player.Character)
            {
                case Automaton:
                {
                    var display = NSequenceDisplay.Create(creature);
                    vfxContainer.AddChildSafely(display);
                    AutomatonCmd.RegisterDisplay(creature, display);
                    display.Refresh();

                    if (creatureNode != null)
                    {
                        var topOfPlayer = creatureNode.GetTopOfHitbox();
                        var localPos = vfxContainer.GetGlobalTransform().AffineInverse() * topOfPlayer;
                        display.Position = localPos + Vector2.Up * 80f + Vector2.Right * 100f;
                    }

                    break;
                }
                case Awakened:
                {
                    var rng = creature.CombatState!.RunState.Rng.CombatCardSelection;
                    AwakenedCmd.GetSpellbook(player)?.Refresh(player, creature.CombatState, rng);
                    
                    TaskHelper.RunSafely(PowerCmd.Apply<AwakenMeterPower>(creature, 1, creature, null, silent: true));
                    
                    var spellbookDisplay = NSpellbookDisplay.Create(player);
                    vfxContainer.AddChildSafely(spellbookDisplay);
                    AwakenedCmd.RegisterDisplay(player, spellbookDisplay);
                    spellbookDisplay.Refresh();

                    if (creatureNode == null) continue;
                    var topOfPlayer = creatureNode.GetTopOfHitbox();
                    var localPos = vfxContainer.GetGlobalTransform().AffineInverse() * topOfPlayer;
                    spellbookDisplay.Position = localPos + Vector2.Up * 80f + Vector2.Left * 120f;
                    break;
                }
            }
        }
    }
}