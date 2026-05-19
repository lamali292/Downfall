using BaseLib.Utils;
using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Rare;

public sealed class Adapt : HermitCardModel
{
    public Adapt() : base(3, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        WithPower<AdaptPower>(1, false);
        WithCostUpgradeBy(-1);
        WithTip(CardKeyword.Exhaust);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.ApplySelf<AdaptPower>(ctx, this);
    }
}