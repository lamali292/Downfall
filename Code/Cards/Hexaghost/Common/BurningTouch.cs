using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Powers.Hexaghost;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Hexaghost.Common;

[Pool(typeof(HexaghostCardPool))]
public class BurningTouch : HexaghostCardModel
{
    public BurningTouch() : base(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithPower<SoulBurnPower>(8, 2);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        var cond = cardPlay.Target.HasPower<SoulBurnPower>();
        await MyCommonActions.Apply<SoulBurnPower>(ctx, this, cardPlay);
        if (!cond) return;
        await MyCommonActions.Apply<SoulBurnPower>(ctx, this, cardPlay);
    }
}