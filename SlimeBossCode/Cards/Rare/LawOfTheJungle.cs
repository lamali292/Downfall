using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Extensions;
using SlimeBoss.SlimeBossCode.Powers;

namespace SlimeBoss.SlimeBossCode.Cards.Rare;

[Pool(typeof(SlimeBossCardPool))]
public class LawOfTheJungle : SlimeBossCardModel
{
    public LawOfTheJungle() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithPower<PotencyPower>(2, 1);
        WithTip<StrengthPower>();
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var stealAmount = DynamicVars.Power<PotencyPower>().IntValue;
        foreach (var slime in Owner.Slimes)
        {
            var potency = Math.Min(slime.GetPowerAmount<PotencyPower>(), stealAmount);
            await PowerCmd.Apply<PotencyPower>(ctx, slime, -potency, Owner.Creature, this);
            await PowerCmd.Apply<StrengthPower>(ctx, Owner.Creature, potency, Owner.Creature, this);
        }
    }
}
