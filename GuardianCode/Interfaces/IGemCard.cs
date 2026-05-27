using BaseLib.Abstracts;
using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.Interfaces;

public interface IGemSocketCard
{
    virtual int GemSlots => 0;
    virtual int GemReplayCount => 1;

    IReadOnlyList<GemModel> Gems =>
        CardModifier.Modifiers(this as CardModel).OfType<GemModel>().ToList();

    int GemCount => Gems.Count;
    int FreeSlots => Math.Max(0, GemSlots - Gems.Count);

    private bool IsFull => Gems.Count >= GemSlots;

    bool CanAddGem(GemModel gem)
    {
        return !IsFull;
    }

    void AddGem(GemModel gem)
    {
        if (IsFull) return;
        var mutableGem = gem.IsMutable ? gem : gem.ToMutable();
        CardModifier.AddModifier(this as CardModel, mutableGem);
    }

    void AddGems(IEnumerable<GemModel> gems)
    {
        foreach (var gem in gems)
        {
            if (IsFull) break;
            AddGem(gem);
        }
    }
}