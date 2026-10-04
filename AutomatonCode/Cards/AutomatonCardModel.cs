using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Compile;
using Automaton.AutomatonCode.DynamicVars;
using Automaton.AutomatonCode.Encode;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Extensions;
using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Automaton.AutomatonCode.Cards;

/// <summary>
///     Base of every Automaton card. A card is an Encode card when it carries the Encode keyword and a
///     Compile card when it carries the Compile keyword; <see cref="WithEncode{T}" /> and
///     <see cref="WithCompile{T}" /> add the keyword and register the effect in one call. The interfaces only
///     expose the registered effects.
/// </summary>
public abstract class AutomatonCardModel(
    int cost,
    CardType type,
    CardRarity rarity,
    TargetType targetType,
    bool showInCardLibrary = true,
    bool autoAdd = true)
    : DownfallCardModel<Core.Automaton>(cost, type, rarity, targetType, showInCardLibrary, autoAdd), IEncodable,
        ICompilable
{
    private readonly List<Compilable> _compilations = [];
    private readonly List<Encodable> _encodings = [];

    public IEnumerable<Encodable> Encodings => _encodings;
    public IEnumerable<Compilable> Compilations => _compilations;


    protected void WithEncode<T>() where T : Encodable, new()
    {
        WithKeyword(AutomatonKeyword.Encode);
        _encodings.Add(new T());
    }

    /// <summary>Adds a Compile effect and the Compile keyword.</summary>
    protected void WithCompile<T>() where T : Compilable, new()
    {
        WithKeyword(AutomatonKeyword.Compile);
        _compilations.Add(new T());
    }

    protected void WithStash(int baseValue, int upgradeValue = 0)
    {
        WithVars(new StashVar(baseValue).WithUpgrade(upgradeValue));
    }
}