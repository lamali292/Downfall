using BaseLib.Utils;
using Downfall.DownfallCode.Powers;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Common;

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
        await CommonActions.Apply<SoulBurnPower>(ctx, this, cardPlay);
        if (!cond) return;
        await CommonActions.Apply<SoulBurnPower>(ctx, this, cardPlay);
    }
}