using BaseLib.Utils;
using Downfall.DownfallCode.Powers;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Rare;

[Pool(typeof(HexaghostCardPool))]
public class Cauterize : HexaghostCardModel
{
    public Cauterize() : base(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(6, 2);
        WithTip(typeof(SoulBurnPower));
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        var attack = await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        var amount = attack.Results.SelectMany(r => r).Sum(x => x.TotalDamage);
        await CommonActions.Apply<SoulBurnPower>(ctx, cardPlay.Target, this, amount);
    }
}