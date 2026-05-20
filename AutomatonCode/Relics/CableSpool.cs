using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Events;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Relics;

[Pool(typeof(AutomatonRelicPool))]
public class CableSpool : AutomatonRelicModel, IOnEncode
{
    private bool _usedThisCombat = false;
    public CableSpool() : base(RelicRarity.Uncommon)
    {

    }

    public override Task BeforeCombatStart()
    {
        _usedThisCombat = false;
        Status = RelicStatus.Active;
        return Task.CompletedTask;
    }


    public async Task OnCardEncoded(PlayerChoiceContext ctx, CardModel encodedCard, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner || _usedThisCombat)
            return;
        Flash();
        _usedThisCombat = true;
        Status = RelicStatus.Normal;

        var copy = encodedCard.CreateClone();
        await CardPileCmd.AddGeneratedCardToCombat(copy, PileType.Hand, Owner);
        await AutomatonCmd.EncodeCard(copy, ctx, cardPlay);
    }
}