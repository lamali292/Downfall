using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Awakened.AwakenedCode.Cards.Rare;

[Pool(typeof(AwakenedCardPool))]
public class Magicianism : AwakenedCardModel
{
    public Magicianism() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithTip(StaticHoverTip.Block);
        this.WithPower<MagicianismPower>(2, 1, false);
    }

    protected override Artist? Artist => Artist.Get<Chimedragon>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<MagicianismPower>(ctx, this);
    }
}