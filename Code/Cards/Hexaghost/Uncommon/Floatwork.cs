using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Powers.Downfall;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Hexaghost.Uncommon;

[Pool(typeof(HexaghostCardPool))]
public class Floatwork : HexaghostCardModel
{
    public Floatwork() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithAfterlife();
        WithPower<DexterityPower>(1);
        WithPower<MetallicizePower>(2, 2);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<DexterityPower>(ctx, this);
        await AfterlifeEffect(ctx, cardPlay);
    }

    protected override async Task AfterlifeEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<MetallicizePower>(ctx, this);
    }
}