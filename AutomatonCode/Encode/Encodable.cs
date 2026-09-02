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
    public static readonly IEnumerable<Encodable> All =
    [
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
    ];

    public abstract TargetType Target { get; }
    public abstract CardType Type { get; }

    private string Id => StringHelper.Slugify(GetType().Name);
    private LocString Description => new("encode", GetType().GetPrefix() + Id + ".encode");
    public abstract DynamicVar FunctionDynamicVar { get; }
    public abstract Task OnPlay(AbstractModel model, PlayerChoiceContext ctx, Creature? target, CardPlay? cardPlay);
    public abstract DynamicVar DynamicVar(AbstractModel card);

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
        DynamicVar(functionCard).BaseValue += EnchantedBase(DynamicVar(sourceCard), sourceCard);
    }


    private static decimal EnchantedBase(DynamicVar v, CardModel card)
    {
        var e = card.Enchantment;
        if (e == null) return v.BaseValue;
        switch (v)
        {
            case DamageVar d:
            {
                var val = d.BaseValue + e.EnchantDamageAdditive(d.BaseValue, d.Props);
                return val * e.EnchantDamageMultiplicative(val, d.Props);
            }
            case BlockVar b:
            {
                var val = b.BaseValue + e.EnchantBlockAdditive(b.BaseValue);
                return val * e.EnchantBlockMultiplicative(val);
            }
            default:
                return v.BaseValue;
        }
    }
}