using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Core.Guardian;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Guardian.Common;

[Pool(typeof(GuardianCardPool))]
public class RollAttack : GuardianCardModel
{
    public RollAttack() : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(16, 4);
        WithBrace(8);
    }

    public override int GemSlots => 1;

    public override TargetType TargetType => !IsMutable
        ? TargetType.AnyEnemy
        : GuardianModel.IsInMode<GuardianDefensiveMode>(Owner)
            ? TargetType.AllEnemies
            : TargetType.AnyEnemy;

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        if (GuardianModel.IsInMode<GuardianDefensiveMode>(Owner)) return;
        await GuardianCmd.Brace(this);
    }
}