using Automaton.AutomatonCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Automaton.AutomatonCode.Encode;

public class FrontloadEncode() : SimpleBlockEncode(8, 3);
public class BranchBlockEncode() : SimpleBlockEncode(7, 2);
public class SafeguardEncode() : SimpleBlockEncode(4, 2);

public abstract class SimpleBlockEncode(decimal baseValue, decimal upgradeValue) : EncodeModifier
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(baseValue, ValueProp.Move)];
    
    public override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (Owner == null) return;
        await CreatureCmd.GainBlock(Owner.Owner.Creature, DynamicVars.Block, cardPlay);
    }

    public override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(upgradeValue);
    }
}