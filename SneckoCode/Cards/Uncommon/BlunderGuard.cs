using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Powers;

namespace Snecko.SneckoCode.Cards.Uncommon;

[Pool(typeof(SneckoCardPool))]
public class BlunderGuard : SneckoCardModel
{
    public BlunderGuard() : base(0, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<BlunderGuardPower>(6, 2, false);
        WithVar("BlunderGuardTwoPower", 1, 1);
        WithTip(StaticHoverTip.Block);
        WithTip<StrengthPower>();
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        (await CommonActions.ApplySelf<BlunderGuardPower>(ctx, this))?.IncrementStrength(
            DynamicVars["BlunderGuardTwoPower"].BaseValue);
    }
}