using System.Linq.Expressions;
using System.Reflection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.DownfallCode.Compatibility;

public static class CompatibilityHook
{
    private static readonly ModifyDamageDel ModifyDamageD = Build();

    public static decimal ModifyDamage(
        IRunState runState, ICombatState? combatState, Creature? target, Creature? dealer,
        decimal damage, ValueProp props, CardModel? cardSource, CardPlay? cardPlay,
        ModifyDamageHookType modifyDamageHookType, CardPreviewMode previewMode,
        out IEnumerable<AbstractModel> modifiers)
    {
        return ModifyDamageD(runState, combatState, target, dealer, damage, props, cardSource,
            cardPlay, modifyDamageHookType, previewMode, out modifiers);
    }

    private static ModifyDamageDel Build()
    {
        var outType = typeof(IEnumerable<AbstractModel>).MakeByRefType();

        Type[] newSig =
        [
            typeof(IRunState), typeof(ICombatState), typeof(Creature), typeof(Creature),
            typeof(decimal), typeof(ValueProp), typeof(CardModel), typeof(CardPlay),
            typeof(ModifyDamageHookType), typeof(CardPreviewMode), outType
        ];

        // V107 signature = same minus CardPlay (index 7)
        Type[] oldSig = [.. newSig[..7], .. newSig[8..]];

        var method = typeof(Hook).GetMethod("ModifyDamage",
                         BindingFlags.Public | BindingFlags.Static, null, newSig, null)
                     ?? typeof(Hook).GetMethod("ModifyDamage",
                         BindingFlags.Public | BindingFlags.Static, null, oldSig, null)
                     ?? throw new MissingMethodException("Hook.ModifyDamage not found in any known signature.");

        var hasCardPlay = method.GetParameters().Length == newSig.Length;

        // Lambda always has the full new-style parameter list, incl. the by-ref 'modifiers'.
        var ps = new[]
        {
            Expression.Parameter(typeof(IRunState), "runState"),
            Expression.Parameter(typeof(ICombatState), "combatState"),
            Expression.Parameter(typeof(Creature), "target"),
            Expression.Parameter(typeof(Creature), "dealer"),
            Expression.Parameter(typeof(decimal), "damage"),
            Expression.Parameter(typeof(ValueProp), "props"),
            Expression.Parameter(typeof(CardModel), "cardSource"),
            Expression.Parameter(typeof(CardPlay), "cardPlay"),
            Expression.Parameter(typeof(ModifyDamageHookType), "hookType"),
            Expression.Parameter(typeof(CardPreviewMode), "previewMode"),
            Expression.Parameter(outType, "modifiers") // by-ref param, passed straight through
        };

        var callArgs = hasCardPlay ? ps : [.. ps[..7], .. ps[8..]];
        var call = Expression.Call(method, callArgs.Cast<Expression>());

        return Expression.Lambda<ModifyDamageDel>(call, ps).Compile();
    }

    private delegate decimal ModifyDamageDel(
        IRunState runState, ICombatState? combatState, Creature? target, Creature? dealer,
        decimal damage, ValueProp props, CardModel? cardSource, CardPlay? cardPlay,
        ModifyDamageHookType modifyDamageHookType, CardPreviewMode previewMode,
        out IEnumerable<AbstractModel> modifiers);
}