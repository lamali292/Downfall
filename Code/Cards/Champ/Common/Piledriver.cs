using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Extensions;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Champ.Common;

[Pool(typeof(ChampCardPool))]
public class Piledriver : ChampCardModel
{
    public Piledriver() : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(8, 4);
        WithPower<VulnerablePower>(2);
        WithPower<WeakPower>(2);
        WithTags(DownfallTag.Finisher);
        WithTip(DownfallTip.Finisher);
    }

    protected override bool ShouldGlowRedInternal => Owner.ChampStance().HasFinisher;
    protected override bool IsPlayable => Owner.ChampStance().HasFinisher;

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;

        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await CommonActions.Apply<VulnerablePower>(ctx,cardPlay.Target, this);
        await CommonActions.Apply<WeakPower>(ctx,cardPlay.Target, this);
        await ChampCmd.PlayFinisher(ctx, cardPlay);
    }
}