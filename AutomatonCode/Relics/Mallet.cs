using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Piles;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Relics;

[Pool(typeof(AutomatonRelicPool))]
public class Mallet : AutomatonRelicModel
{
    public Mallet() : base(RelicRarity.Rare)
    {
        WithTip(AutomatonTip.Stash);
    }

    public override Task AfterCardChangedPiles(CardModel card, PileType oldPileType,
        AbstractModel? clonedBy)
    {
        if (card.Owner != Owner || card.Pile?.Type != StashPile.Stash) return Task.CompletedTask;
        CardCmd.Upgrade(card);
        return Task.CompletedTask;
    }
}