using Awakened.AwakenedCode.Cards.Token;
using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Events;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Awakened.AwakenedCode.Relics;

[Pool(typeof(AwakenedRelicPool))]
public class ZenerDeck : AwakenedRelicModel, IModifyBaseSpells
{
    public ZenerDeck() : base(RelicRarity.Rare)
    {
        WithTip(typeof(ESP));
    }
    
    public IReadOnlyList<Type> ModifyBaseSpells(Player owner, IReadOnlyList<Type> types)
        => [..types, typeof(ESP)];
}