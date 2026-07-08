using Automaton.AutomatonCode.Cards.Rare;
using Automaton.AutomatonCode.Cards.Token;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Automaton.AutomatonCode.Encode;

public abstract class Encodable
{
    public abstract TargetType Target { get; }
    public abstract CardType Type { get; }

    private string Id =>  StringHelper.Slugify(GetType().Name);
    private LocString Description => new("encode", GetType().GetPrefix() +  Id+".encode");
    public abstract Task OnPlay(AbstractModel model, PlayerChoiceContext ctx, Creature? target, CardPlay? cardPlay);
    public abstract DynamicVar DynamicVar(AbstractModel card);
    public abstract DynamicVar FunctionDynamicVar { get; }
    public virtual IEnumerable<IHoverTip> HoverTips(AbstractModel card) => [];

    public LocString GetDescription(AbstractModel card)
    {
        var description = Description;
        card.GetDynamicVars().AddTo(description);
        return description;
    }

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

    public void ApplyEncode(FunctionCard functionCard, CardModel sourceCard)
    {
        DynamicVar(functionCard).BaseValue += DynamicVar(sourceCard).BaseValue;
    }
}