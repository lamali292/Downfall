using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Snecko.SneckoCode.Events;

public interface IAfterCardMuddled
{
    Task AfterCardMuddled(PlayerChoiceContext ctx, CardModel card, AbstractModel? source);
}