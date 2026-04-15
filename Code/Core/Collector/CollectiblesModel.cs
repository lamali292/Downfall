using System;
using System.Collections.Generic;
using System.Linq;
using BaseLib.Utils;
using Downfall.Code.Nodes;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace Downfall.Code.Core.Collector;

public static class CollectiblesModel
{
    
    
    // TODO - ??????????? what did  i do here, help
    public static readonly SavedSpireField<RelicModel, SerializableCard[]> Collectibles = new(Array.Empty<SerializableCard>, "Downfall_Collector_Collectibles");

    public static List<CardModel> GetCollectibles(Player player)
    {
        var relic = StartingRelic(player);
        var saved = Collectibles.Get(relic) ?? [];
        return saved.Select(CardModel.FromSerializable).ToList();
    }

    public static void AddCollectible(Player player, CardModel card)
    {
        var relic = StartingRelic(player);
        var current = Collectibles.Get(relic) ?? [];
        var next = new SerializableCard[current.Length + 1];
        Array.Copy(current, next, current.Length);
        next[current.Length] = card.ToSerializable();
        Collectibles.Set(relic, next);
        NTopBarCollectorButton.RefreshButton();
    }

    private static RelicModel StartingRelic(Player player)
    {
        var startingRelics = player.Character.StartingRelics;
        if (startingRelics.Count == 0) 
            throw new Exception("No starting relic defined for character.");
        var relicId = startingRelics[0].Id;
        var actualRelic = player.GetRelicById(relicId);
        return actualRelic ?? throw new Exception("No relic with id " + relicId);
    }
}

