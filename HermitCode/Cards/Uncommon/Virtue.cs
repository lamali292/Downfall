using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Uncommon;

public sealed class Virtue : HermitCardModel
{
    public Virtue() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeyword(CardKeyword.Retain);
        WithVar("Reduce", 1, 1);
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        var reduceBy = DynamicVars["Reduce"].IntValue;
        foreach (var power in Owner.Creature.Powers
                     .Where(p => p.StackType == PowerStackType.Counter)
                     .Where(p => p.TypeForCurrentAmount == PowerType.Debuff)
                     .ToList())
        {
            var change = power.Amount > 0
                ? -Math.Min(reduceBy, power.Amount)
                : Math.Min(reduceBy, Math.Abs(power.Amount));
            await PowerCmd.ModifyAmount(ctx, power, change, Owner.Creature, null);
        }
    }
}