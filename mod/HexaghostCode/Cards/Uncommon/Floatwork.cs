using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Hexaghost.HexaghostCode.Cards.Uncommon;

[Pool(typeof(HexaghostCardPool))]
public class Floatwork : HexaghostCardModel, IHasEarlyAfterlifeEffect
{
    public Floatwork() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithAfterlife();
        WithPower<DexterityPower>(1, 1);
        WithPower<PlatingPower>(3);
    }

    protected override Artist Artist => Artist.Get<Thelethargicweirdo>();

    public async Task AfterlifeEffect(PlayerChoiceContext ctx, CardPlay? cardPlay, bool wasExhausted,
        bool causedByEthnereal)
    {
        await CommonActions.ApplySelf<PlatingPower>(ctx, this);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<DexterityPower>(ctx, this);
        await AfterlifeEffect(ctx, cardPlay, false, false);
    }
}