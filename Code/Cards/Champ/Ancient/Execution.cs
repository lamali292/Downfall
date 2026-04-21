using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Extensions;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Champ.Ancient;

[Pool(typeof(ChampCardPool))]
public class Execution : ChampCardModel
{
    public Execution() : base(2, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy)
    {
        WithDamage(6, 3);
        WithTags(DownfallTag.Finisher);
        WithTip(DownfallTip.Finisher);
    }

    protected override bool ShouldGlowRedInternal => Owner.ChampStance().HasFinisher;
    protected override bool IsPlayable => Owner.ChampStance().HasFinisher;

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).WithHitCount(3).Execute(ctx);
        await ChampCmd.PlayFinisher(ctx, cardPlay, true, 2);
    }
}