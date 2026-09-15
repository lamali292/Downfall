using BaseLib.Abstracts;
using Downfall.DownfallCode.Interfaces;
using Godot;
using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.Interfaces;

public interface IGemSocketCard : IModifyReplayCount, ICardOverlay
{
    int GemSlots { get; }
    int GemReplayCount => 1;
    bool GemsAffectAllPlayers => false;
    bool ShouldPlayGems => true;

    IReadOnlyList<GemModel> Gems =>
        this is CardModel card
            ? CardModifier.Modifiers(card).OfType<GemModel>().ToList()
            : throw new InvalidOperationException();

    int GemCount => Gems.Count;
    int FreeSlots => Math.Max(0, GemSlots - Gems.Count);

    /// <summary>
    /// Gems beyond GemSlots (e.g. left over after a downgrade shrinks capacity below what's
    /// socketed) stay attached but don't play - matching the socket display, which only ever
    /// draws the first GemSlots gems. They aren't lost: if GemSlots grows again (a re-upgrade,
    /// CryoChamber, ...) they become active again automatically, since this is just a live slice
    /// of the full Gems list rather than a separate tracked state.
    /// </summary>
    IEnumerable<GemModel> ActiveGems => Gems.Take(GemSlots);

    private bool IsFull => Gems.Count >= GemSlots;

    Control ICardOverlay.CreateCustomOverlay()
    {
        return new CardGemDisplay();
    }

    void ICardOverlay.UpdateOverlay(Control overlay)
    {
        ((CardGemDisplay)overlay).Refresh(this);
    }

    int IModifyReplayCount.ModifyReplayCount(int current)
    {
        return ActiveGems.Aggregate(current, (c, gem) => gem.ModifyPlayCount(c));
    }

    bool CanAddGem(GemModel gem)
    {
        return !IsFull;
    }

    void AddGem(GemModel gem)
    {
        if (IsFull || this is not CardModel card) return;
        var mutableGem = gem.IsMutable ? gem : gem.ToMutable();
        CardModifier.AddModifier(card, mutableGem);
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