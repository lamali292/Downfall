using Automaton.AutomatonCode.Cards.Token;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Encode;

/// <summary>
///     An <see cref="Encodable" /> with a number: the source cards' values are summed into one var on the
///     Function, and the effect plays whenever that var is above 0.
/// </summary>
public abstract class ValueEncode : Encodable
{
    public abstract TargetType Target { get; }
    public abstract CardType Type { get; }

    /// <summary>Fixed play order: effects fire in ascending order, independent of source-card order.</summary>
    public abstract int Order { get; }

    /// <summary>Playing the Function stops after this effect resolves (Full Release defers everything else to its Power).</summary>
    public virtual bool EndsSequence => false;

    /// <summary>The Function targets Self whatever the other effects want.</summary>
    public virtual bool ForcesSelfTarget => false;

    /// <summary>Whether a Function carrying this effect counts as gaining Block.</summary>
    public virtual bool GainsBlock => false;

    private LocString Description => new("encode", GetType().GetPrefix() + Id + ".encode");

    /// <summary>
    ///     The single definition of this effect's var. A fresh instance is the Function's var; a card or
    ///     power that carries this effect owns a var with the same name, which <see cref="DynamicVar" /> finds.
    /// </summary>
    public abstract DynamicVar FunctionDynamicVar { get; }

    private string? _varName;
    private string VarName => _varName ??= FunctionDynamicVar.Name;

    /// <summary>The var this effect reads on <paramref name="model" /> (a source card, a Function or a power).</summary>
    public DynamicVar DynamicVar(AbstractModel model)
    {
        return model.DynamicVars[VarName];
    }

    public override LocString GetDescription(AbstractModel card)
    {
        var description = Description;
        description.Add("IsOnCard", card is CardModel and not FunctionCard);
        description.Add("IsOnFunction", card is FunctionCard);
        description.Add("IsOnPower", card is PowerModel);
        card.DynamicVars.AddTo(description);
        return description;
    }

    public override void ApplyEncode(FunctionCard function, CardModel sourceCard, FunctionPosition position)
    {
        DynamicVar(function).BaseValue += EnchantedBase(sourceCard);
    }

    /// <summary>
    ///     The source card's value merged into the Function. Effects whose var enchantments can modify
    ///     (Block, Damage) override this to fold the enchantment in; everything else uses the plain base value.
    /// </summary>
    protected virtual decimal EnchantedBase(CardModel sourceCard)
    {
        return DynamicVar(sourceCard).BaseValue;
    }
}
