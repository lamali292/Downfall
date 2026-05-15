using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Powers;

namespace SlimeBoss.SlimeBossCode.Cards.Common;

[Pool(typeof(SlimeBossCardPool))]
public class GoopSpray : SlimeBossCardModel
{
    public GoopSpray() : base(1, CardType.Skill, CardRarity.Common, TargetType.AllEnemies)
    {
        WithPower<GoopPower>(5, 3);
        WithPower<WeakPower>(1);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await MyCommonActions.Apply<GoopPower>(ctx, this, cardPlay);
        await MyCommonActions.Apply<WeakPower>(ctx, this, cardPlay);
    }
}