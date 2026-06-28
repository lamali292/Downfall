using Automaton.AutomatonCode.Core;
using BaseLib.Extensions;
using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Automaton.AutomatonCode.Encode;


public class PhilosophizeEncode() : SimplePowerEncode<StrengthPower>(1, 1, TargetType.Self);
public class InvalidateEncode() : SimplePowerEncode<VulnerablePower>(1, 1, TargetType.AnyEnemy);
public class DeprecateEncode() : SimplePowerEncode<WeakPower>(1, 1, TargetType.AnyEnemy);
public class ExplodeEncode() : SimplePowerEncode<SoulBurnPower>(15, 5, TargetType.AllEnemies);

public abstract class SimplePowerEncode<T>(decimal baseValue, decimal upgradeValue, TargetType targetType) : EncodeModifier
where T : PowerModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<T>(baseValue)];
    
    public override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        switch (targetType)
        {
            case TargetType.Self:
                await MyCommonActions.ApplySelf<T>(ctx, this);
                break;
            case TargetType.AllEnemies:
                if (cardPlay.Card.CombatState == null) return;
                await CommonActions.Apply<SoulBurnPower>(ctx, cardPlay.Card.CombatState.HittableEnemies, this);
                break;
            case TargetType.AnyEnemy:
                if (cardPlay.Target == null) return;
                await CommonActions.Apply<SoulBurnPower>(ctx, cardPlay.Target, this);
                break;
            case TargetType.RandomEnemy:
                if (cardPlay.Card.CombatState == null) return;
                var target = Owner?.RunState?.Rng.CombatTargets.NextItem(cardPlay.Card.CombatState.HittableEnemies);
                if (target == null) return;
                await CommonActions.Apply<SoulBurnPower>(ctx, target, this);
                break;
        }
       
    }

    public override void OnUpgrade()
    {
        DynamicVars.Power<T>().UpgradeValueBy(upgradeValue);
    }
}