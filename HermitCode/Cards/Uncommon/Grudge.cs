using BaseLib.Utils;
using Downfall.DownfallCode.Extensions;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hermit.HermitCode.Cards.Uncommon;

public sealed class Grudge : HermitCardModel
{
    public Grudge() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithCalculatedDamage(9, 2, CountCurses, ValueProp.Move, 0, 1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await CommonActions.CardAttack(this, play)
            .WithHermitFireHitFx()
            .Execute(ctx);
    }

    private static decimal CountCurses(CardModel card, Creature? _)
    {
        return card.Owner.GetAllCards().Count(e => e.Type == CardType.Curse);
    }
}