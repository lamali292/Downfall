using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Awakened.AwakenedCode.Cards.Rare;

[Pool(typeof(AwakenedCardPool))]
public class Magicianism : AwakenedCardModel
{
    public Magicianism() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithTip(StaticHoverTip.Block);
        WithPower<MagicianismPower>(2, 1, false);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<MagicianismPower>(ctx, this);
    }
}