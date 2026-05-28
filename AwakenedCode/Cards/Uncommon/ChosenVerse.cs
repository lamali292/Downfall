using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Powers;
using BaseLib.Extensions;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Awakened.AwakenedCode.Cards.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class ChosenVerse : AwakenedCardModel
{
    public ChosenVerse() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        this.WithPower<ChosenVersePower>(4, 2, false);
        WithTip(StaticHoverTip.Block);
    }
    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var power = await CommonActions.ApplySelf<ChosenVersePower>(ctx, this, 2);
        if (power == null) return;
        power.SetBlock(DynamicVars.Power<ChosenVersePower>().IntValue);
        power.CardPlay = cardPlay;
    }
}