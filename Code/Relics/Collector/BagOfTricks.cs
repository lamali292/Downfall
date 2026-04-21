using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Relics.Collector;

[Pool(typeof(CollectorRelicPool))]
public class BagOfTricks : CollectorRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Common;

    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        CombatState combatState)
    {
        if (player != Owner || combatState.RoundNumber > 1) return;
        await CollectorCmd.DrawCollected(choiceContext, player, 2);
        Flash();
    }
}