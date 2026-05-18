using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Ancient;

/// <summary>
///     Ethereal. All your cards cost 0. You can no longer gain Energy. At the start of your turn, add a random Curse to
///     your hand.
///     Upgrade: Remove Ethereal.
/// </summary>
public sealed class EternalSentence : HermitCardModel
{
    public EternalSentence() : base(3, CardType.Power, CardRarity.Ancient, TargetType.Self)
    {
        WithKeyword(CardKeyword.Ethereal);
        WithKeyword(CardKeyword.Ethereal, UpgradeType.Remove);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<EternalSentencePower>(ctx, Owner.Creature, 1, Owner.Creature, this);
    }
}

/* transform_cards.py changes:
 *   namespace → Hermit.HermitCode.Cards.Ancient
 *   usings updated
 *   CanonicalKeywords removed → WithKeyword(...) in constructor
 *   OnUpgrade removed (all logic migrated to constructor)
 *   constructor: WithKeyword(CardKeyword.Ethereal), WithKeyword(CardKeyword.Ethereal, UpgradeType.Remove)
 */