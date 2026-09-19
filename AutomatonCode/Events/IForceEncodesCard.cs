using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Events;

/// <summary>
///     Implemented by relics/powers that force a card to be Encoded on play even though the card
///     itself isn't normally Encodable (e.g. Platinum Core force-encoding the owner's basic
///     Strikes/Defends). Checked via <see cref="AutomatonHook.WillForceEncode" /> so other
///     Encode-pile-aware effects (Bronze Orb, Summon Orb) don't fight over the same card play.
/// </summary>
public interface IForceEncodesCard
{
    bool ForceEncodes(CardModel card);
}
