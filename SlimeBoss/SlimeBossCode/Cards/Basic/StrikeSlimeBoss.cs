using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Cards.Basic;

[Pool(typeof(SlimeBossCardPool))]
public class StrikeSlimeBoss : SlimeBossCardModel
{
    public StrikeSlimeBoss() : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
        WithTags(CardTag.Strike);
    }

    // TODO: Implement
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
    }


}