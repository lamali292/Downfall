using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using BaseLib.Abstracts;
using BaseLib.Utils;
using Gremlins.GremlinsCode.Vfx;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace Gremlins.GremlinsCode.Core;

public class GremlinsRunModel() : CustomSingletonModel(HookType.Run)
{
    public static SavedSpireField<Player, List<GremlinSaveData>> GremlinStats =
        new(() => [], "GremlinStats")
        {
            Serializer = (list, writer) =>
            {
                writer.WriteInt(list.Count);
                foreach (var g in list) g.Serialize(writer);
            },
            Deserializer = reader =>
            {
                var count = reader.ReadInt();
                var list = new List<GremlinSaveData>(count);
                for (var i = 0; i < count; i++)
                {
                    var g = new GremlinSaveData();
                    g.Deserialize(reader);
                    list.Add(g);
                }

                return list;
            }
        };

    private static CustomMonsterModel[]? _startingGremlins;

    private static readonly ConditionalWeakTable<Player, GremlinState> States = new();

    public static CustomMonsterModel[] StartingGremlins => _startingGremlins ??=
    [
        ModelDb.Monster<ShieldGremlin>(),
        ModelDb.Monster<MadGremlin>(),
        ModelDb.Monster<FatGremlin>(),
        ModelDb.Monster<SneakGremlin>(),
        ModelDb.Monster<WizardGremlin>()
    ];

    public static GremlinState GetState(Player player)
    {
        if (States.TryGetValue(player, out var state)) return state;
        state = new GremlinState();
        States.Add(player, state);
        return state;
    }

    public override Task AfterActEntered()
    {
        var runState = RunManager.Instance.State;
        if (runState is not { ActFloor: 1 }) return Task.CompletedTask;
        foreach (var player in runState.Players)
            if (player.Character is Gremlins)
                GremlinStats.Set(player, StartingGremlins.Select(m => new GremlinSaveData
                {
                    ModelId = m.Id,
                    Hp = m.MaxInitialHp,
                    MaxHp = m.MaxInitialHp
                }).ToList());

        return Task.CompletedTask;
    }

    public override Task BeforeCombatStart()
    {
        var combatState = CombatManager.Instance._state;
        if (combatState == null) return Task.CompletedTask;

        foreach (var player in combatState.Players)
        {
            if (player.Character is not Gremlins) continue;
            if (player.PlayerCombatState == null) continue;

            var state = GetState(player);
            state.Reset();
            var saveData = GremlinStats.Get(player);
            if (saveData == null) continue;
            foreach (var saved in saveData)
            {
                var model = ModelDb.GetById<MonsterModel>(saved.ModelId);
                GremlinsCmd.AddGremlin(player, model, saved.Hp, saved.MaxHp);
            }

            var creatureNode = NCombatRoom.Instance?.GetCreatureNode(player.Creature);
            if (creatureNode?.Visuals is NGremlinsCreatureVisuals visuals)
                visuals.ArrangeGremlins(state.Gremlins);

            var active = state.Active;
            if (active == null) continue;
            player.Creature.SetMaxHpInternal(active.MaxHp);
            player.Creature.SetCurrentHpInternal(active.CurrentHp);
        }


        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        var combatState = CombatManager.Instance._state;
        if (combatState == null) return base.AfterCombatEnd(room);

        foreach (var player in combatState.Players)
        {
            if (player.Character is not Gremlins) continue;
            var state = GetState(player);
            GremlinStats.Set(player, state.ToSaveData(player));
            state.Reset();
        }

        return Task.CompletedTask;
    }
}

public class GremlinSaveData : IPacketSerializable
{
    [JsonPropertyName("model_id")] public ModelId ModelId { get; set; } = ModelId.none;
    [JsonPropertyName("hp")] public int Hp { get; set; }
    [JsonPropertyName("max_hp")] public int MaxHp { get; set; }

    public void Serialize(PacketWriter writer)
    {
        writer.WriteFullModelId(ModelId);
        writer.WriteInt(Hp);
        writer.WriteInt(MaxHp);
    }

    public void Deserialize(PacketReader reader)
    {
        ModelId = reader.ReadFullModelId();
        Hp = reader.ReadInt();
        MaxHp = reader.ReadInt();
    }
}