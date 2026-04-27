using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Champ.ChampCode.Cards.Uncommon;

[Pool(typeof(ChampCardPool))]
public class FaceSlap : ChampCardModel
{
    public FaceSlap() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(8, 2);
        WithPower<VulnerablePower>(2, 1);
    }

    protected override bool ShouldGlowGoldInternal => Owner.ShouldBerserkerComboTrigger();

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        if (!Owner.ShouldBerserkerComboTrigger() || cardPlay.Target == null) return;
        await CommonActions.Apply<VulnerablePower>(ctx, cardPlay.Target, this);
    }
}