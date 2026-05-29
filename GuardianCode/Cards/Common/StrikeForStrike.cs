using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Cards.Common;

[Pool(typeof(GuardianCardPool))]
public class StrikeForStrike : GuardianCardModel, IGemSocketCard
{
    public StrikeForStrike() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(13, 4);
        this.WithEnemyDamage(3);
        WithTags(CardTag.Strike);
    }

    public int GemSlots => 1;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await DownfallCmd.EnemyAttackPlayer(ctx, cardPlay, this);
    }
}