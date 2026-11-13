using Downfall.DownfallCode.Compatibility;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Events;
using Guardian.GuardianCode.Piles;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.Powers;

public class ReroutePower : GuardianPowerModel, IModifyCardPlayResultLocation
{
    private CardModel? _cardSource;


    public CardLocationCompatiblity ModifyCardPlayResultLocationCompability(CardModel card, bool isAutoPlay,
        ResourceInfo resources, CardLocationCompatiblity cardLocation)
    {
        var player = card.Owner;
        if (_cardSource == card || card.Keywords.Contains(CardKeyword.Exhaust) ||
            card is not { Type: CardType.Attack or CardType.Skill } || player.Creature != Owner)
            return cardLocation;

        // This decision runs before the card's own OnPlay effect (see CardModel.OnPlayWrapper),
        // so a card that independently stasis'd another card during its own effect (e.g. Curl Up)
        // wouldn't be reflected in a plain pile count check here. GetEffectiveStasisCount also
        // counts cards already committed to Stasis by an earlier redirect this play.
        if (GuardianCombatModel.GetEffectiveStasisCount(player) >= GuardianCmd.GetMaxStasisSlots(player))
            return cardLocation;

        GuardianCombatModel.PendingStasisRedirect[player] = card;
        return new CardLocationCompatiblity(card.Owner, GuardianPile.Stasis, CardPilePosition.Bottom);
    }

    public async Task AfterModifyingCardPlayResultLocationCompability(CardModel card,
        CardLocationCompatiblity cardLocation)
    {
        // This only fires when we actually redirected the card into Stasis (see
        // HookUtils.Modify in ModifyCardPlayResultLocationPatch), and the vanilla move has
        // already happened by this point. Unlike GuardianCmd.PutIntoStasis, this path never
        // dispatches the Stasis-entry hooks itself, so relics like CryoChamber (upgrade on
        // entry) and QuantumChamber (after entry) never fired for a Reroute-stashed card.
        await GuardianHook.BeforeCardEntersStasis(card.Owner, card, this);
        GuardianCmd.SetStasisCounter(card);
        await GuardianHook.AfterCardEntersStasis(card.Owner, card, this);
        card.EnergyCost.AfterCardPlayedCleanup();
        await PowerCmd.Decrement(this);
    }

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        _cardSource = cardSource;
        return Task.CompletedTask;
    }


    public override Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        return PowerCmd.Remove(this);
    }
}