using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Snecko.SneckoCode.Cards.Token;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Relics;

[Pool(typeof(SneckoRelicPool))]
public class SneckoSoul : SneckoRelicModel
{
    public SneckoSoul() : base(RelicRarity.Starter)
    {
        WithTip<SoulRoll>();
    }

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (player != Owner || Owner.PlayerCombatState is not { TurnNumber: 1 }) return;
        await DownfallCardCmd.GiveCard<SoulRoll>(player, PileType.Hand);
    }


    public override RelicModel GetUpgradeReplacement()
    {
        return ModelDb.Relic<SuperSneckoSoul>();
    }
}