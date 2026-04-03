using Downfall.Code.Character;
using Downfall.Code.Nodes;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Downfall.Code.Core;

public class GremlinsRunModel : AbstractModel
{
    private static readonly CustomMonsterModel[] FormModels =
    [
        ModelDb.Monster<ShieldGremlin>(),
        ModelDb.Monster<AngryGremlin>(),
        ModelDb.Monster<FatGremlin>(),
        ModelDb.Monster<SneakGremlin>(),
        ModelDb.Monster<WizardGremlin>()
    ];

    private static readonly Dictionary<Player, GremlinState> _states = new();
    public override bool ShouldReceiveCombatHooks => true;

    public static GremlinState GetState(Player player)
    {
        if (_states.TryGetValue(player, out var state)) return state;
        state = new GremlinState();
        _states[player] = state;

        return state;
    }

    public static List<Creature>? GetGremlins(Player player)
    {
        return _states.TryGetValue(player, out var s) ? s.Gremlins : null;
    }

    public static int GetActiveIndex(Player player)
    {
        return _states.TryGetValue(player, out var s) ? s.ActiveIndex : 0;
    }


    public override Task BeforeCombatStart()
    {
        var combatState = CombatManager.Instance.DebugOnlyGetState();
        if (combatState == null) return Task.CompletedTask;


        foreach (var player in combatState.Players)
        {
            if (player.Character is not Gremlins) continue;

            var state = new GremlinState();
            _states[player] = state;

            if (player.PlayerCombatState == null) continue;
            foreach (var model in FormModels)
            {
                var mutable = model.ToMutable();
                var creature = combatState.CreateCreature(mutable, CombatSide.Player, null);
                mutable.SetUpForCombat();
                creature.PetOwner = player;
                player.PlayerCombatState.AddPetInternal(creature);
                state.Gremlins.Add(creature);

                // Spawn a real NCreature node
                NCombatRoom.Instance!.AddCreature(creature);
            }

// Now arrange them
            var creatureNode = NCombatRoom.Instance?.GetCreatureNode(player.Creature);
            if (creatureNode?.Visuals is NGremlinsCreatureVisuals visuals)
                visuals.ArrangeGremlins(state.Gremlins);
        }

        return Task.CompletedTask;
    }

    public class GremlinState
    {
        public List<Creature> Gremlins { get; } = [];
        public int ActiveIndex { get; set; }

        public Creature? Active => Gremlins.ElementAtOrDefault(ActiveIndex);

        public bool HasLivingGremlins => Gremlins.Any(g => g.IsAlive);

        public int GetNextLivingIndex()
        {
            var count = Gremlins.Count;
            for (var i = 1; i < count; i++)
            {
                var next = (ActiveIndex + i) % count;
                if (Gremlins[next].IsAlive) return next;
            }

            return -1;
        }
    }
}