using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Hexaghost.HexaghostCode.Cards.Uncommon;

[Pool(typeof(HexaghostCardPool))]
public class RainOfEmbers : HexaghostCardModel
{
    public RainOfEmbers() : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(6, 3);
        WithPower<WeakPower>(1);
    }

    protected override bool HasEnergyCostX => true;

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var x = ResolveEnergyXValue();
        for (var i = 0; i < x; i++) {
            await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
            await MyCommonActions.Apply<WeakPower>(ctx, this, cardPlay);
        }
    }
}