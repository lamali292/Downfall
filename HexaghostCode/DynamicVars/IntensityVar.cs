using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Events;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Hexaghost.HexaghostCode.DynamicVars;

public class GhostflameVar(string name, decimal baseValue) : DynamicVar(name, baseValue)
{
    public int IntensityValue => IntValue + (_owner is GhostflameModel ghostflameModel
        ? HexaghostHook.ModifyGhostflameEffectAdditive(ghostflameModel.Owner.Creature.CombatState!,
            ghostflameModel.Owner, ghostflameModel)
        : 0);

    public void UpdateGhostflamePreview(
        GhostflameModel ghostflameModel,
        bool runGlobalHooks)
    {
        var originalDamage1 = BaseValue;
        if (runGlobalHooks)
            originalDamage1 += HexaghostHook.ModifyGhostflameEffectAdditive(ghostflameModel.Owner.Creature.CombatState!,
                ghostflameModel.Owner, ghostflameModel);
        PreviewValue = originalDamage1;
    }

    public override string ToString()
    {
        return $"{PreviewValue}";
    }
}

public class GhostflameDamageVar(decimal baseValue) : GhostflameVar("Damage", baseValue);

public class GhostflameBlockVar(decimal baseValue) : GhostflameVar("Block", baseValue);

public class GhostflameSoulburnVar(decimal baseValue) : GhostflameVar("Soulburn", baseValue);