using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Powers.Hexaghost;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Hexaghost.Rare;

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
           var attack = await CommonActions.CardAttack(this,  cardPlay).Execute(ctx);
           var amount = attack.Results.Sum(e => e.TotalDamage);
           await CommonActions.Apply<SoulBurnPower>(ctx, cardPlay.Target, this, amount);
    }
}