using BaseLib.Patches.Features;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Powers;

namespace SlimeBoss.SlimeBossCode.Cards.Common;

[Pool(typeof(SlimeBossCardPool))]
public class FormOfWall : SlimeBossCardModel
{
    public FormOfWall() : base(2, CardType.Skill, CardRarity.Common, CustomTargetType.AllAttackingEnemies)
    {
        WithBlock(12, 3);
        WithPower<GoopPower>(4, 2);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.Apply<GoopPower>(ctx, this, cardPlay);
    }
}