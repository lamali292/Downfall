using BaseLib.Utils;
using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Uncommon;

public sealed class Brawl : HermitCardModel
{
    public Brawl() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<BruisePower>(3, 2);
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.ApplySelf<BrawlPower>(ctx, this);
    }
}