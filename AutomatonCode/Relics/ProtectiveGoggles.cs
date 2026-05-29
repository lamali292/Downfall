using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Extensions;
using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Relics;

[Pool(typeof(AutomatonRelicPool))]
public class ProtectiveGoggles : AutomatonRelicModel
{
    public ProtectiveGoggles() : base(RelicRarity.Common)
    {
        WithBlock(4);
        WithTip(AutomatonTip.Encode);
    }

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext ctx, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner.Creature)) return;
        if (Owner.GetEncode().Count == 0) return;
        Flash();
        await MyCommonActions.Block(this);
    }
}