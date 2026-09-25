using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Gremlins.GremlinsCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Gremlins.GremlinsCode.Cards.Common;

[Pool(typeof(GremlinsCardPool))]
public class Pretaliation : GremlinsCardModel
{
    public Pretaliation() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(6, 2);
        this.WithRepeat(2);
        this.WithEnemyDamage(3, -2);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay, DynamicVars.Repeat.IntValue).Execute(ctx);
        await DownfallCombatCmd.EnemyAttackPlayer(ctx, cardPlay, this);
    }
}