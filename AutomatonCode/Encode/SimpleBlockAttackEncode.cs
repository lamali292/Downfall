using Automaton.AutomatonCode.Core;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Automaton.AutomatonCode.Encode;

public class FragmentEncode() : SimpleBlockAttackEncode(3, 1, 3, 1);
public class DazingPulseEncode() : SimpleBlockAttackEncode(7, 2, 7, 2);
public class NullPointerEncode() : SimpleBlockAttackEncode(12, 3, 12, 3);

public abstract class SimpleBlockAttackEncode(decimal baseBlockValue, decimal upgradeBlockValue, decimal baseAttackValue, decimal upgradeAttackValue) : EncodeModifier
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(baseAttackValue, ValueProp.Move),
            new BlockVar(baseBlockValue, ValueProp.Move)];
    
    public override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (Owner == null) return;
        await CreatureCmd.GainBlock(Owner.Owner.Creature, DynamicVars.Block, cardPlay);
        if (cardPlay.Target == null) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCardCompatibility(Owner, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(ctx);
    }
    
    public override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(upgradeBlockValue);
        DynamicVars.Damage.UpgradeValueBy(upgradeAttackValue);
    }
}