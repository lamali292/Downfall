using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Powers.Hexaghost;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Hexaghost.Basic;

[Pool(typeof(HexaghostCardPool))]
public class Sear : HexaghostCardModel
{
    public Sear() : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
        WithAfterlife();
        WithDamage(5, 2);
        WithPower<SoulBurnPower>(5, 2);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await AfterlifeEffect(ctx, cardPlay);
    }

    protected override  async Task AfterlifeEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await MyCommonActions.Apply<SoulBurnPower>(ctx, this, cardPlay);
    }
}