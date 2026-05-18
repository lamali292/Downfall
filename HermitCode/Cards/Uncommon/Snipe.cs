using BaseLib.Utils;
using Hermit.HermitCode.CustomEnums;
using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Hermit.HermitCode.Cards.Uncommon;

/// <summary>
///     Your next Dead On effect this turn triggers twice. Exhaust.
///     Upgrade: Also gain 1 Concentrate.
/// </summary>
public sealed class Snipe : HermitCardModel
{
    public Snipe() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.None)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithKeyword(HermitKeywords.Concentrate, UpgradeType.Add);
        WithPower<ConcentrationPower>(0, 1, false);
        WithPower<SnipePower>(1, false);
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.ApplySelf<SnipePower>(ctx, this);
        await CommonActions.ApplySelf<ConcentrationPower>(ctx, this);
    }
}

/* transform_cards.py changes:
 *   namespace → Hermit.HermitCode.Cards.Uncommon
 *   usings updated
 *   CanonicalKeywords removed → WithKeyword(...) in constructor
 *   OnUpgrade removed (all logic migrated to constructor)
 *   constructor: WithKeyword(CardKeyword.Exhaust)
 */