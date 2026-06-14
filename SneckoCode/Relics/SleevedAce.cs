using BaseLib.Utils;
using Downfall.DownfallCode.Abstract;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using Snecko.SneckoCode.Cards.Uncommon;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Relics;

[Pool(typeof(SneckoRelicPool))]
public class SleevedAce : SneckoRelicModel
{
    public SleevedAce() : base(RelicRarity.Uncommon)
    {
        WithTip(new RelicTooltipSource(_ =>
        {
            var tip = ModelDb.Card<MarkedCard>().ToMutable();
            tip.UpgradeInternal();
            return HoverTipFactory.FromCard(tip);
        }));
    }

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (player != Owner || Owner.PlayerCombatState is not { TurnNumber: 1 }) return;
        await DownfallCardCmd.GiveCard<MarkedCard>(player, PileType.Hand, upgraded: true);
    }
}