using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Awakened.AwakenedCode.Cards.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class DarkEcho : AwakenedCardModel
{
    public DarkEcho() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithTip(typeof(StrengthPower));
        WithPower<DarkEchoPower>(1, false);
        WithCostUpgradeBy(-1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<DarkEchoPower>(ctx, this);
    }
}