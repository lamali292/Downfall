using BaseLib.Utils;
using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Guardian.GuardianCode.Gems;

public class RutileGem : GemModel
{
    public override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var combatState = cardPlay.Card.CombatState;
        if (combatState == null) return;
        await PowerCmd.Apply<WeakPower>(ctx, combatState.HittableEnemies, 1, cardPlay.Card.Owner.Creature, cardPlay.Card);
    }
}