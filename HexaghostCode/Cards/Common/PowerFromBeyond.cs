using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Hexaghost.HexaghostCode.Cards.Common;

[Pool(typeof(HexaghostCardPool))]
public class PowerFromBeyond : HexaghostCardModel, IHasAfterlifeEffect
{
    public PowerFromBeyond() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithAfterlife();
        WithBlock(3, 2);
        WithEnergy(2);
        WithPower<EnergyNextTurnPower>(2, false);
    }

    protected override Artist Artist => Artist.Get<Thelethargicweirdo>();


    public async Task AfterlifeEffect(PlayerChoiceContext ctx, CardPlay? cardPlay, bool wasExhausted,
        bool causedByEthereal)
    {
        await CommonActions.CardBlock(this, cardPlay);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await AfterlifeEffect(ctx, cardPlay, false, false);
        await CommonActions.ApplySelf<EnergyNextTurnPower>(ctx, this);
    }
}