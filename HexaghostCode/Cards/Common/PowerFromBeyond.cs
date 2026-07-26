using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Extensions;
using Hexaghost.HexaghostCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Hexaghost.HexaghostCode.Cards.Common;

[Pool(typeof(HexaghostCardPool))]
public class PowerFromBeyond : HexaghostCardModel, IHasAfterlifeEffect
{
    //todo Ethereal. Afterlife. **Next turn, draw 1 card.** Next turn, gain [E][E]([E]).
    public PowerFromBeyond() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        this.WithAfterlife();
        WithPower<VigorPower>(3, 1);
        WithEnergy(2, 1);
        this.WithPower<EnergyNextTurnPower>(2, 1, false);
    }

    protected override Artist Artist => Artist.Get<Thelethargicweirdo>();


    public async Task AfterlifeEffect(PlayerChoiceContext ctx, CardPlay? cardPlay, bool wasExhausted)
    {
        await CommonActions.ApplySelf<VigorPower>(ctx, this);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await AfterlifeEffect(ctx, cardPlay, false);
        await CommonActions.ApplySelf<EnergyNextTurnPower>(ctx, this);
    }
}