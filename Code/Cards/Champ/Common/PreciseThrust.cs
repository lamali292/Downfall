using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Champ.Common;

[Pool(typeof(ChampCardPool))]
public class PreciseThrust : ChampCardModel
{
    public PreciseThrust() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(6, 2);
        WithBlock(6, 2);
    }

    protected override bool ShouldGlowGoldInternal =>
        Owner.ShouldBerserkerComboTrigger() || Owner.ShouldDefensiveComboTrigger();

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay.Target).Execute(ctx);

        if (Owner.ShouldBerserkerComboTrigger())
            await CommonActions.CardAttack(this, cardPlay.Target).Execute(ctx);
        if (Owner.ShouldDefensiveComboTrigger())
            await CommonActions.CardBlock(this, cardPlay);
    }
}