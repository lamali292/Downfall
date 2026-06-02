using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Powers;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Hexaghost.HexaghostCode.Cards.Rare;

[Pool(typeof(HexaghostCardPool))]
public class TurnItUp : HexaghostCardModel
{
    public TurnItUp() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        this.WithPower<TemporaryStrengthUpPower>(2, 1, false);
        this.WithPower<TemporaryDexterityUpPower>(2, 1, false);
        this.WithPower<TemporaryIntensityPower>(2, 1, false);
        this.WithTip<StrengthPower>();
        this.WithTip<DexterityPower>();
        this.WithTip<IntensityPower>();
        WithKeyword(CardKeyword.Retain);
    }

    protected override Artist Artist => Artist.Get<CartesianCanvas>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<TemporaryStrengthUpPower>(ctx, this);
        await CommonActions.ApplySelf<TemporaryDexterityUpPower>(ctx, this);
        await CommonActions.ApplySelf<TemporaryIntensityPower>(ctx, this);
    }
}