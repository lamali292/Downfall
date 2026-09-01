using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Powers;

public class MulliganPower : SneckoPowerModel
{
    private static bool CostMoreThanNormal(CardPlay? play)
    {
        if (play?.Card == null) return false;
        if (play.Card.EnergyCost.CostsX) return false;
        return play.Resources.EnergyValue > play.Card.EnergyCost.GetWithModifiers(default);
    }


    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var player = cardPlay.Card.Owner;
        if (player.Creature != Owner) return;
        if (!CostMoreThanNormal(cardPlay)) return;
        await PowerCmd.Apply<EnergyNextTurnPower>(ctx, player.Creature, Amount, player.Creature, null);
        Flash();
    }
}