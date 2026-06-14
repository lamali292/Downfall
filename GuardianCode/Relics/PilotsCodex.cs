using BaseLib.Utils;
using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.Relics;

[Pool(typeof(GuardianRelicPool))]
public class PilotsCodex() : GuardianRelicModel(RelicRarity.Rare)
{
    public override async Task BeforeHandDrawLate(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (player != Owner || Owner.PlayerCombatState is not { TurnNumber: 1 }) return;
        var cards = ModelDb.CardPool<GuardianCardPool>()
            .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint);
        var list = CardFactory.GetDistinctForCombat(Owner, cards, 2, Owner.RunState.Rng.CombatCardGeneration).ToList();
        Flash();
        foreach (var card in list)
        {
            await CardPileCmd.Add(card, PileType.Play);
            await GuardianCmd.PutIntoStasis(card, ctx, this, true);
        }
    }
}