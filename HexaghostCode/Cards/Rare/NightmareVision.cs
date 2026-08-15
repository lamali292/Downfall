using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Hexaghost.HexaghostCode.Cards.Rare;

[Pool(typeof(HexaghostCardPool))]
public class NightmareVision : HexaghostCardModel
{
    public NightmareVision() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        this.WithPower<NightmareVisionPower>(4, 2, false);
        WithTip(StaticHoverTip.Block);
        WithTip(CardKeyword.Ethereal);
        WithTip(CardKeyword.Exhaust);
    }

    protected override Artist Artist => Artist.Get<Thelethargicweirdo>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<NightmareVisionPower>(ctx, this);
    }
}