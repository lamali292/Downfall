using Downfall.Code.Abstract;
using Downfall.Code.Interfaces;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Powers.Awakened;

public class MoonlitVisionPower : AwakenedPowerModel, IHasSecondAmount
{
    private int SpellsPlayedThisTurn => CombatManager.Instance.History.Entries
        .OfType<CardPlayStartedEntry>()
        .Count(e => e.HappenedThisTurn(CombatState) &&
                    e.CardPlay.Card is ISpell &&
                    e.CardPlay.Card.Owner == Owner.Player);

    public string GetSecondAmount()
    {
        return $"{Math.Max(Amount - SpellsPlayedThisTurn, 0)}";
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player) return;
        if (cardPlay.Card is not ISpell) return;

        InvokeDisplayAmountChanged();

        var previousSpellsThisTurn = SpellsPlayedThisTurn - 1;
        if (previousSpellsThisTurn < Amount)
        {
            Flash();
            await PlayerCmd.GainEnergy(1, Owner.Player);
        }
    }

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature == Owner) InvokeDisplayAmountChanged();

        return Task.CompletedTask;
    }
}