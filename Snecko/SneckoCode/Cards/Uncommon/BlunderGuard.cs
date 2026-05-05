using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Powers;

namespace Snecko.SneckoCode.Cards.Uncommon;

[Pool(typeof(SneckoCardPool))]
public class BlunderGuard : SneckoCardModel
{
    public BlunderGuard() : base(0, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithPower<BlunderGuardPower>(6, 2);
        WithPower<BlunderGuardTwoPower>(2, 1);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.ApplySelf<BlunderGuardPower>(ctx, this);
        await CommonActions.ApplySelf<BlunderGuardTwoPower>(ctx, this);
    }
}