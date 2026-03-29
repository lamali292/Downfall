using Downfall.Code.Cards.Vfx;
using Downfall.Code.Character;
using Downfall.Code.Commands;
using Downfall.Code.Displays;
using Downfall.Code.Powers.Awakened;
using Downfall.Code.Vfx;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;

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
            //if (!LocalContext.IsMe(player)) continue;

            var creature = player.Creature;
            var combatRoom = NCombatRoom.Instance;
            if (combatRoom == null) continue;

            var creatureNode = combatRoom.GetCreatureNode(creature);
            var vfxContainer = combatRoom.CombatVfxContainer;

            switch (player.Character)
            {
                case Automaton:
                {
                    var display = NSequenceDisplay.Create(player);
                    vfxContainer.AddChildSafely(display);
                    AutomatonDisplay.Register(player, display);
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
                    AwakenedCmd.GetSpellbook(player)?.Refresh(player);
                    TaskHelper.RunSafely(PowerCmd.Apply<AwakenMeterPower>(creature, 1, creature, null, true));

                    var spellbookDisplay = NSpellbookDisplay.Create(player);
                    vfxContainer.AddChildSafely(spellbookDisplay);
                    AwakenedDisplay.Register(player, spellbookDisplay);
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