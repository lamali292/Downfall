using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Cards.Token;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.CustomEnums;

namespace Snecko.SneckoCode.Relics;

[Pool(typeof(SneckoRelicPool))]
public class SuperSneckoSoul : SneckoRelicModel
{
    public SuperSneckoSoul() : base(RelicRarity.Starter)
    {
        WithTip(SneckoKeywords.Muddle);
    }


    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (player != Owner) return;
        await DownfallCardCmd.GiveCard<SoulRoll>(player, PileType.Hand);
        if (player.PlayerCombatState is { TurnNumber: 1 }) return;
        var card = await CardPileCmd.Draw(ctx, Owner);
        if (card == null) return;
        await SneckoCmd.Muddle(ctx, card, this);
    }
}