using BaseLib.Utils;
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
    public PowerFromBeyond() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        this.WithAfterlife();
        WithPower<VigorPower>(3, 1);
        WithEnergy(2, 1);
        WithPower<EnergyNextTurnPower>(2, 1);
    }


    public async Task AfterlifeEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<VigorPower>(ctx, this);
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await AfterlifeEffect(ctx, cardPlay);
        await CommonActions.ApplySelf<EnergyNextTurnPower>(ctx, this);
    }
}