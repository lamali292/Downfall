using BaseLib.Utils;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Awakened.Token;

[Pool(typeof(TokenCardPool))]
public class Ceremony() : AwakenedCardModel(0, CardType.Power, CardRarity.Token, TargetType.None)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Retain
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<StrengthPower>(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<StrengthPower>()
    ];

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<StrengthPower>(this, DynamicVars["StrengthPower"].BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["StrengthPower"].UpgradeValueBy(1);
    }
}