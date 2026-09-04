using Collector.CollectorCode.Cards.Basic;
using Collector.CollectorCode.Cards.Rare;
using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Powers;

public class DarkLordFormPlusPower : CollectorPowerModel
{

    public DarkLordFormPlusPower()
    {
        WithUpgradedCardTip<YouAreMine>();
        WithTips(e => e.Amount > 1 ? [HoverTipFactory.Static(StaticHoverTip.ReplayStatic)] : []);
    }
    
    public override async Task BeforeHandDrawLate(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (player.Creature != Owner) return;
        var card = player.Creature.CombatState!.CreateCard(ModelDb.Card<YouAreMine>(), player);
        card.UpgradeInternal();
        if (Amount > 1) card.BaseReplayCount = Amount - 1;
        card.IsDupe = true;
        await CardCmd.AutoPlay(ctx, card, null);
    }
}