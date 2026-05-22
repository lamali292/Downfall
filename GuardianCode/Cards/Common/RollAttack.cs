using BaseLib.Utils;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Cards.Common;

[Pool(typeof(GuardianCardPool))]
public class RollAttack : GuardianCardModel
{
    public RollAttack() : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(16, 4);
        WithBrace(8);
        WithTip(GuardianTip.DefensiveMode);
    }

    public override int GemSlots => 1;
    protected override bool ShouldGlowGoldInternal => GuardianCmd.IsInMode<GuardianDefensiveMode>(Owner);

    public override TargetType TargetType => _owner == null || !IsMutable
        ? TargetType.AnyEnemy
        : GuardianCmd.IsInMode<GuardianDefensiveMode>(Owner)
            ? TargetType.AllEnemies
            : TargetType.AnyEnemy;

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        if (GuardianCmd.IsInMode<GuardianDefensiveMode>(Owner)) return;
        await GuardianCmd.Brace(ctx, this);
    }
}