using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Events;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace Automaton.AutomatonCode.Relics;

[Pool(typeof(AutomatonRelicPool))]
public class CableSpool : AutomatonRelicModel
{
    private int _usesLeft = 2;


    public override bool ShowCounter => CombatManager.Instance.IsInProgress;
    public override int DisplayAmount => _usesLeft;

    
    public CableSpool() : base(RelicRarity.Uncommon)
    {
        WithCards(2);
        WithTip(AutomatonTip.Encode);
    }

    public override Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Owner != Owner || !AutomatonCmd.IsEncodable(card) || _usesLeft == 0) return Task.CompletedTask;
        CardCmd.Upgrade(card);
        _usesLeft--;
        InvokeDisplayAmountChanged();
        Flash();
        return Task.CompletedTask;
    }
    public override Task BeforeCombatStart()
    {
        _usesLeft = 2;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
}