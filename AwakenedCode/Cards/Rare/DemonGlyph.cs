using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.CustomEnums;
using Awakened.AwakenedCode.Powers;
using BaseLib.Extensions;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Awakened.AwakenedCode.Cards.Rare;

[Pool(typeof(AwakenedCardPool))]
public class DemonGlyph : AwakenedCardModel
{
    public DemonGlyph() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithPower<StrengthPower>(1);
        WithPower<DexterityPower>(1);
        this.WithPower<DemonGlyphPower>(2, 1, false);
        WithTip(AwakenedTip.Awaken);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<StrengthPower>(ctx, this);
        await CommonActions.ApplySelf<DexterityPower>(ctx, this);
        if (AwakenedModel.IsAwakened(Owner))
        {
            var count = DynamicVars.Power<DemonGlyphPower>().BaseValue;
            await CommonActions.ApplySelf<StrengthPower>(ctx, this, count);
            await CommonActions.ApplySelf<DexterityPower>(ctx, this, count);
        }
        else
        {
            await CommonActions.ApplySelf<DemonGlyphPower>(ctx, this);
        }
    }
}