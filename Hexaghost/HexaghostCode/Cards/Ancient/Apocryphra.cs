using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Ancient;

[Pool(typeof(HexaghostCardPool))]
public class Apocryphra : HexaghostCardModel
{
    public Apocryphra() : base(1, CardType.Attack, CardRarity.Ancient, TargetType.AllEnemies)
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

    protected override async Task AfterlifeEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await MyCommonActions.Apply<SoulBurnPower>(ctx, this, cardPlay);
        await CardPileCmd.Add(this, PileType.Hand);
    }
}