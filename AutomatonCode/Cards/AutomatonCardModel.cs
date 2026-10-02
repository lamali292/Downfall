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
public abstract class AutomatonCardModel : DownfallCardModel<Core.Automaton>, IEncodable, ICompilable
{
    private readonly List<Compilable> _compilations = [];
    private readonly List<Encodable> _encodings = [];
    private bool _hasEncodeKeyword;

    protected AutomatonCardModel(
        int cost,
        CardType type,
        CardRarity rarity,
        TargetType targetType,
        bool showInCardLibrary = true,
        bool autoAdd = true
    ) : base(cost, type, rarity, targetType, showInCardLibrary, autoAdd)
    {
    }

    public IEnumerable<Encodable> Encodings => _encodings;
    public IEnumerable<Compilable> Compilations => _compilations;

    /// <summary>
    ///     Adds an Encode effect. The card gets the Encode keyword (it can be put into a Function and is encoded
    ///     when played) unless <paramref name="encodesOnPlay" /> is false: a card that only has the effects and
    ///     is encoded by something else (Strike and Defend with Platinum Core) is granted the keyword when it
    ///     enters the Encode pile.
    /// </summary>
    protected void WithEncode<T>(bool encodesOnPlay = true) where T : Encodable, new()
    {
        if (encodesOnPlay && !_hasEncodeKeyword)
        {
            WithKeyword(AutomatonKeyword.Encode);
            _hasEncodeKeyword = true;
        }

        _encodings.Add(new T());
    }

    /// <summary>Adds a Compile effect and the Compile keyword.</summary>
    protected void WithCompile<T>() where T : Compilable, new()
    {
        if (_compilations.Count == 0) WithKeyword(AutomatonKeyword.Compile);
        _compilations.Add(new T());
    }

    protected void WithStash(int baseValue, int upgradeValue = 0)
    {
        WithVars(new StashVar(baseValue).WithUpgrade(upgradeValue));
    }
}