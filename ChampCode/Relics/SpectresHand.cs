using BaseLib.Utils;
using Champ.ChampCode.Cards.Basic;
using Champ.ChampCode.Core;
using Champ.ChampCode.Events;
using Champ.ChampCode.Stance;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Champ.ChampCode.Relics;

[Pool(typeof(ChampRelicPool))]
public class SpectresHand() : ChampRelicModel(RelicRarity.Rare), IOnChampStanceChange
{
    public async Task OnChampStanceChange(PlayerChoiceContext ctx, Player player, ChampStanceModel oldStance,
        ChampStanceModel newStance)
    {
        if (player != Owner || newStance is ChampNoStance || oldStance == newStance) return;
        CardModel card;
        if (player.RunState.Rng.CombatCardGeneration.NextBool())
            card = await DownfallCardCmd.GiveCard<StrikeChamp>(player, PileType.Hand);
        else
            card = await DownfallCardCmd.GiveCard<DefendChamp>(player, PileType.Hand);
        card.ToEcho();
        card.SetToFreeThisTurn();
    }
}