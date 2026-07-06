using Automaton.AutomatonCode.Core;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Automaton.AutomatonCode.Encode;

public class ReplicateEncode() : SimpleAttackEncode(5, 2);
public class SpikeEncode() : SimpleAttackEncode(4, 1);
public class MinorBeamEncode() : SimpleAttackEncode(4, 2);
public class BacktraceEncode() : SimpleAttackEncode(7, 2);
public class BranchAttackEncode() : SimpleAttackEncode(7, 2);
public class DigitalCarnageEncode() : SimpleAttackEncode(20, 8);
public class InfiniteLoopEncode() : SimpleAttackEncode(6);


public abstract class SimpleAttackEncode(decimal baseValue, decimal upgradeValue = 0) : EncodeModifier
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(baseValue, ValueProp.Move)];
    
    public override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Target == null || Owner == null) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCardCompatibility(Owner, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(ctx);
    }


    public override void OnUpgrade()
    {
        if (upgradeValue != 0)
            DynamicVars.Damage.UpgradeValueBy(upgradeValue);
    }
}