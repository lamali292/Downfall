using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Awakened.AwakenedCode.Cards.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class Spellshield : AwakenedCardModel
{
    public Spellshield() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithTip(CardKeyword.Retain);
        WithTip(StaticHoverTip.Block);
        this.WithPower<SpellshieldPower>(2, 1, false);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<SpellshieldPower>(ctx, this);
    }
}