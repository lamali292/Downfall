using BaseLib.Utils;
using Downfall.DownfallCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Uncommon;

public sealed class Scavenge : HermitCardModel, IHasDeadOnEffect
{
    public Scavenge() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
    {
        WithPower<PlatedArmorPower>(4, 1);
        WithKeyword(CardKeyword.Exhaust);
        WithGold(5, 5);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.ApplySelf<PlatedArmorPower>(ctx, this);
    }

    public async Task DeadOnEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await PlayerCmd.GainGold(DynamicVars.Gold.BaseValue, Owner);
    }
}