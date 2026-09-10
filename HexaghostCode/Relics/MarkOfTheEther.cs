using BaseLib.Utils;
using Downfall.DownfallCode.Abstract;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.CustomEnums;
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
    public MarkOfTheEther() : base(RelicRarity.Starter)
    {
        WithBlock(4);
        WithTip(HexaghostTip.Ignite);
    }
    
    
    public async Task AfterGhostflameIgnited(PlayerChoiceContext ctx, Player player, GhostflameModel flame, int index)
    {
        if (player != Owner) return;
        Flash();
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.IntValue, BlockProps.nonCardUnpowered, null, true);
    }
}