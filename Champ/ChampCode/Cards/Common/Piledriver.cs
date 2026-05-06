using BaseLib.Utils;
using Champ.ChampCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Champ.ChampCode.Cards.Common;

[Pool(typeof(ChampCardPool))]
public class Piledriver : ChampCardModel
{
    public Piledriver() : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(8, 4);
        WithPower<VulnerablePower>(2);
        WithPower<WeakPower>(2);
        WithFinisher();
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;

        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await CommonActions.Apply<VulnerablePower>(ctx, cardPlay.Target, this);
        await CommonActions.Apply<WeakPower>(ctx, cardPlay.Target, this);
        await ChampCmd.PlayFinisher(ctx, cardPlay);
    }
}