using BaseLib.Utils;
using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Rare;

public sealed class HeroicBravado : HermitCardModel
{
    public HeroicBravado() : base(1, CardType.Skill, CardRarity.Rare, TargetType.None)
    {
        WithKeyword(CardKeyword.Ethereal);
        WithPower<RuggedPower>(1);
        WithVar("CostIncrease", 2, -1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.ApplySelf<RuggedPower>(ctx, this);
        EnergyCost.AddThisCombat(DynamicVars["CostIncrease"].IntValue);
    }
}