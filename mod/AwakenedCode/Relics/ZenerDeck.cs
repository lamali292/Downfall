using Awakened.AwakenedCode.Cards.Token;
using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Events;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;

namespace Awakened.AwakenedCode.Relics;

[Pool(typeof(AwakenedRelicPool))]
public class ZenerDeck : AwakenedRelicModel, IModifyBaseSpells
{
    public ZenerDeck() : base(RelicRarity.Rare)
    {
        WithTip<ESP>();
    }

    public IReadOnlyList<CardModel> ModifyBaseSpells(Player owner, IReadOnlyList<CardModel> types)
    {
        return [..types, ModelDb.Card<ESP>()];
    }
}