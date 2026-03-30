using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Extensions;
using Downfall.Code.Powers.Awakened;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Awakened.Rare;

[Pool(typeof(AwakenedCardPool))]
public class DemonGlyph : AwakenedCardModel
{
    public DemonGlyph() : base(1, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        WithTip(typeof(StrengthPower));
        WithTip(typeof(DexterityPower));
        WithPower<DemonGlyphPower>(2);
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        // could be done better.
        if (AwakenedCmd.IsAwakened(Owner.Creature))
        {
            var count = DynamicVars.Power<DemonGlyphPower>().BaseValue;
            await CommonActions.ApplySelf<StrengthPower>(this, 1 + count);
            await CommonActions.ApplySelf<DexterityPower>(this, 1 + count);
        }
        else
        {
            await CommonActions.ApplySelf<StrengthPower>(this, 1);
            await CommonActions.ApplySelf<DexterityPower>(this, 1);
            await MyCommonActions.ApplySelf<DemonGlyphPower>(this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<DemonGlyphPower>().UpgradeValueBy(1);
    }
}