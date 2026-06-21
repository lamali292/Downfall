using Awakened.AwakenedCode.Core;
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
        this.WithTip<StrengthPower>();
        this.WithTip<DexterityPower>();
        this.WithPower<DemonGlyphPower>(2, 1, false);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        // could be done better.
        if (AwakenedModel.IsAwakened(Owner))
        {
            var count = DynamicVars.Power<DemonGlyphPower>().BaseValue;
            await CommonActions.ApplySelf<StrengthPower>(ctx, this, 1 + count);
            await CommonActions.ApplySelf<DexterityPower>(ctx, this, 1 + count);
        }
        else
        {
            await CommonActions.ApplySelf<StrengthPower>(ctx, this, 1);
            await CommonActions.ApplySelf<DexterityPower>(ctx, this, 1);
            await CommonActions.ApplySelf<DemonGlyphPower>(ctx, this);
        }
    }
}