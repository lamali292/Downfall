using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Cards.Token;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Powers;

public class SlimeCrushNextTurnPower() : SlimeBossPowerModel(PowerType.Buff, PowerStackType.Single)
{
    public bool CreateUpgraded;

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (player.Creature != Owner) return;
        var card = combatState.CreateCard<SlimeCrush>(player);
        if (CreateUpgraded) card.UpgradeInternal();
        await CardPileCmd.Add(card, PileType.Hand);
        await PowerCmd.Remove(this);
    }
}
