using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;
using MegaCrit.Sts2.Core.Commands;

namespace SlimeBoss.SlimeBossCode.Cards.Common;

[Pool(typeof(SlimeBossCardPool))]
public class HeavyTackle : SlimeBossCardModel
{
    public HeavyTackle() : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithTags(SlimeBossTag.Tackle);
        WithDamage(12, 4);
        WithEnergy(2);
    }

    protected override bool ShouldGlowGoldInternal => SlimeBossCmd.HasPlayedStatusThisTurn(Owner);

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var playedStatus = SlimeBossCmd.HasPlayedStatusThisTurn(Owner);
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        if (playedStatus) await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
    }
}
