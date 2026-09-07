using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Cards.Common;

[Pool(typeof(CollectorCardPool))]
public class FollowThePyre : CollectorCardModel
{
    
    public FollowThePyre() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(5, 3);
        WithPower<FollowThePyrePower>(5, 2, false);
        WithKeyword(CollectorKeyword.Pyre);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }

    public override async Task AfterCardDrawn(PlayerChoiceContext ctx, CardModel card, bool fromHandDraw)
    {
        if (card != this || CombatState == null) return;
        var randomEnemy = RunState?.Rng.CombatTargets.NextItem(CombatState.HittableEnemies);
        if (randomEnemy == null) return;
        await CommonActions.Apply<FollowThePyrePower>(ctx, randomEnemy, this);
    }
}