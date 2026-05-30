using BaseLib.Abstracts;
using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.Interfaces;

public interface IGemSocketCard
{
    virtual int GemSlots => 0;
    virtual int GemReplayCount => 1;

    IReadOnlyList<GemModel> Gems =>
        this is CardModel card ? CardModifier.Modifiers(card).OfType<GemModel>().ToList() : throw new InvalidOperationException();

    int GemCount => Gems.Count;
    int FreeSlots => Math.Max(0, GemSlots - Gems.Count);

    private bool IsFull => Gems.Count >= GemSlots;

    bool CanAddGem(GemModel gem)
    {
        return !IsFull;
    }

    void AddGem(GemModel gem)
    {
        if (IsFull || this is not CardModel card) return;
        var mutableGem = gem.IsMutable ? gem : gem.ToMutable();
        CardModifier.AddModifier(card, mutableGem);
        mutableGem.ApplyOnAddedEffects(card);
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