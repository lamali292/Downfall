using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Interfaces;

public interface IUsesPyredCards
{
    /// <summary>
    /// The cards exhausted to satisfy this card's pyre cost.
    /// For <c>Pyre</c> this is the single hand card chosen to exhaust;
    /// for <c>Megapyre</c> this is every card in the player's hand.
    /// </summary>
    IEnumerable<CardModel> PyredCards { get; set; }
}