using BaseLib.Abstracts;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Hexaghost.HexaghostCode.Cards.Uncommon;

[Pool(typeof(HexaghostCardPool))]
public class Premonition : HexaghostCardModel
{
    public Premonition() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<PremonitionPower>(1, false);
        WithKeyword(CardKeyword.Innate, UpgradeType.Add);
        WithTip(CardKeyword.Exhaust);
        WithTip(CardKeyword.Ethereal);
        WithTip<StrengthPower>();
    }

    protected override Artist Artist => Artist.Get<CartesianCanvas>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<PremonitionPower>(ctx, this);
    }
}