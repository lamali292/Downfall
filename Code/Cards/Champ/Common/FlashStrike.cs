using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Extensions;
using Downfall.Code.Powers.Champ;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Champ.Common;

[Pool(typeof(ChampCardPool))]
public class FlashStrike : ChampCardModel
{
    public FlashStrike() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(6, 3);
        WithBlock(5, 2);
        WithPower<CounterPower>(4, 2);

    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay.Target).Execute(ctx);
        if (!Owner.ShouldDefensiveComboTrigger()) return;
        await CommonActions.ApplySelf<CounterPower>(this);
        await CommonActions.CardBlock(this, cardPlay);
    }
}