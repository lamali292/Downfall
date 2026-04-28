using BaseLib.Utils;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hexaghost.HexaghostCode.Relics;

[Pool(typeof(HexaghostRelicPool))]
public class MarkOfTheEther : HexaghostRelicModel, IAfterGhostflameIgnited
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    
    public async Task AfterGhostflameIgnited(PlayerChoiceContext ctx, Player player, GhostflameModel flame, int index)
    {
        if (player != Owner) return;
        Flash();
        await CreatureCmd.GainBlock(Owner.Creature, 4, ValueProp.Move | ValueProp.Unpowered, null, true);
    }
}