using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hermit.HermitCode.Cards.Common;

public sealed class LowProfile : HermitCardModel
{
    public LowProfile() : base(1, CardType.Skill, CardRarity.Common, TargetType.None)
    {
        WithCalculatedBlock(7, 4, CountDebuffs, ValueProp.Move, 2, 1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await MyCommonActions.CardCalculatedBlock(this, play);
    }

    private static decimal CountDebuffs(CardModel card, Creature? _)
    {
        return card.Owner.Creature.Powers.Count(p => p.TypeForCurrentAmount == PowerType.Debuff);
    }
}