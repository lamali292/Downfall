using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Rare;

/// <summary>
///     First 4 playable cards drawn at the start of each turn cost 1 less that turn.
///     Upgrade: Remove Ethereal.
/// </summary>
public sealed class EternalForm : HermitCardModel
{
    private const int EternalAmount = 4;

    public EternalForm() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithPower<EternalPower>(4);
        WithKeyword(CardKeyword.Ethereal);
        WithKeyword(CardKeyword.Ethereal, UpgradeType.Remove);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<EternalPower>(ctx, Owner.Creature, DynamicVars["EternalPower"].BaseValue, Owner.Creature,
            this);
    }
}

/* transform_cards.py changes:
 *   namespace → Hermit.HermitCode.Cards.Rare
 *   usings updated
 *   CanonicalKeywords removed → WithKeyword(...) in constructor
 *   OnUpgrade removed (all logic migrated to constructor)
 *   constructor: WithPower<EternalPower>(4, 0), WithKeyword(CardKeyword.Ethereal), WithKeyword(CardKeyword.Ethereal, UpgradeType.Remove)
 */