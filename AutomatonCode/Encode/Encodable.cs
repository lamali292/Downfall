using Automaton.AutomatonCode.Cards.Token;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Encode;

public abstract class Encodable
{
    /// <summary>Every encode effect, in play order (<see cref="Order" />).</summary>
    public static readonly IEnumerable<Encodable> All = new Encodable[]
    {
        new PowerEncode(),
        new BlockEncode(),
        new DamageEncode(),
        new StrengthEncode(),
        new WeakEncode(),
        new VulnerableEncode(),
        new PoisonEncode(),
        new SoulburnEncode(),
        new EnergyEncode(),
        new DazedEncode()
    }.OrderBy(e => e.Order).ToList();

    public abstract TargetType Target { get; }
    public abstract CardType Type { get; }

    /// <summary>Loc key part in <c>encode.json</c>: <c>&lt;MOD PREFIX&gt;&lt;Id&gt;.encode</c>. Explicit so renaming the class cannot break loc.</summary>
    public abstract string Id { get; }

    /// <summary>Fixed play order: effects fire in ascending order, independent of source-card order.</summary>
    public abstract int Order { get; }

    private LocString Description => new("encode", GetType().GetPrefix() + Id + ".encode");

    /// <summary>
    ///     The single definition of this effect's var. A fresh instance is the Function's var; a card or
    ///     power that carries this effect owns a var with the same name, which <see cref="DynamicVar" /> finds.
    /// </summary>
    public abstract DynamicVar FunctionDynamicVar { get; }

    public abstract Task OnPlay(AbstractModel model, PlayerChoiceContext ctx, Creature? target, CardPlay? cardPlay);

    private string? _varName;
    private string VarName => _varName ??= FunctionDynamicVar.Name;

    /// <summary>The var this effect reads on <paramref name="model" /> (a source card, a Function or a power).</summary>
    public DynamicVar DynamicVar(AbstractModel model)
    {
        return model.DynamicVars[VarName];
    }

    public virtual IEnumerable<IHoverTip> HoverTips(AbstractModel card)
    {
        return [];
    }

    public LocString GetDescription(AbstractModel card)
    {
        var description = Description;
        description.Add("IsOnCard", card is CardModel and not FunctionCard);
        description.Add("IsOnFunction", card is FunctionCard);
        description.Add("IsOnPower", card is PowerModel);
        card.DynamicVars.AddTo(description);
        return description;
    }

    public void ApplyEncode(FunctionCard functionCard, CardModel sourceCard)
    {
        DynamicVar(functionCard).BaseValue += EnchantedBase(sourceCard);
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
