using BaseLib.Utils;
using Downfall.DownfallCode.Powers;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Rare;

[Pool(typeof(HexaghostCardPool))]
public class Ignis : HexaghostCardModel
{
    public Ignis() : base(1, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithPower<SoulBurnPower>(6, 4);
        WithPower<IgnisPower>(1, false);
        WithTip(CardKeyword.Ethereal);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        await CommonActions.Apply<SoulBurnPower>(ctx, this, cardPlay);
        await CommonActions.ApplySelf<IgnisPower>(ctx, this);
    }
}
