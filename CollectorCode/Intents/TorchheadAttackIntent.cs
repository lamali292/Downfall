using Collector.CollectorCode.Events;
using Collector.CollectorCode.Extensions;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Intents;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;

namespace Collector.CollectorCode.Intents;

/// <summary>
/// Torchhead auto-attacks at the end of its owner's side turn (see TorchheadPower.AfterSideTurnEnd),
/// instead of acting through the normal monster move state machine, so it needs its own intent
/// (same approach as Hexaghost's ghostflame intents) rather than a vanilla AttackIntent.
/// </summary>
public class TorchheadAttackIntent : CustomIntent
{
    public override IntentType IntentType => IntentType.Attack;

    private static int GetDamage(Creature owner)
    {
        var power = owner.GetPower<TorchheadPower>();
        if (power == null) return 0;

        // Same pipeline the power's own tooltip preview uses, so the intent always matches it.
        power.DynamicVars.TorchheadDamage.UpdatePowerPreview(power, CardPreviewMode.None, null, runGlobalHooks: true);
        return (int)power.DynamicVars.TorchheadDamage.PreviewValue;
    }

    private static bool TargetsAllEnemies(Creature owner)
    {
        var petOwner = owner.PetOwner;
        return petOwner != null && CollectorHook.ShouldTorchheadTargetAll(petOwner, out _);
    }

    public override string GetAnimation(IEnumerable<Creature> targets, Creature owner)
    {
        return GetDamage(owner) switch
        {
            <= 5 => IntentAnimData.attack1,
            <= 10 => IntentAnimData.attack2,
            <= 20 => IntentAnimData.attack3,
            <= 40 => IntentAnimData.attack4,
            _ => IntentAnimData.attack5
        };
    }

    public override LocString GetIntentLabel(IEnumerable<Creature> targets, Creature owner)
    {
        var label = new LocString("intents", "FORMAT_DAMAGE_SINGLE");
        label.Add("Damage", GetDamage(owner));
        return label;
    }

    protected override LocString GetIntentDescription(IEnumerable<Creature> targets, Creature owner)
    {
        var description = base.GetIntentDescription(targets, owner);
        description.Add("Damage", GetDamage(owner));
        description.Add("TargetsAll", TargetsAllEnemies(owner));
        return description;
    }
}
