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
using SlimeBoss.SlimeBossCode.CustomEnums;

namespace SlimeBoss.SlimeBossCode.Cards.Rare;

[Pool(typeof(SlimeBossCardPool))]
public class LawOfTheJungle : SlimeBossCardModel
{
    public LawOfTheJungle() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithTip(SlimeBossTip.Split);
        WithPower<PotencyPower>(2);
        WithPower<StrengthPower>(2, 1);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await PowerCmd.Apply<PotencyPower>(ctx, Owner.SlimeCreatures, 
            -DynamicVars.Power<PotencyPower>().BaseValue, Owner.Creature, this);
        await PowerCmd.Apply<StrengthPower>(ctx, Owner.Creature, 
            Owner.SlimeCreatures.Count*DynamicVars.Power<StrengthPower>().BaseValue, Owner.Creature, this);
    }
}
