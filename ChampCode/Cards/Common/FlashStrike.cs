using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Extensions;
using Champ.ChampCode.Interfaces;
using Champ.ChampCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Cards.Common;

[Pool(typeof(ChampCardPool))]
public class FlashStrike : ChampCardModel, IDefensiveComboCard
{
    public FlashStrike() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(6, 3);
        WithBlock(5, 2);
        WithPower<CounterPower>(4, 2);
        WithTags(CardTag.Strike);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay.Target).Execute(ctx);
    }

    public async Task DefensiveComboEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<CounterPower>(ctx, this);
        await CommonActions.CardBlock(this, cardPlay);
    }
}