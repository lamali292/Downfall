using BaseLib.Utils;
using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Guardian.GuardianCode.Cards.Uncommon;

[Pool(typeof(GuardianCardPool))]
public class GuardianWhirl : GuardianCardModel
{
    protected override bool ShouldGlowGoldInternal => Owner.Creature.Block >= DynamicVars["Threshold"].IntValue;

    public GuardianWhirl() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithDamage(4, 2);
        WithVar("Threshold", 16);
        WithTip(StaticHoverTip.Block);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay, Owner.Creature.Block >= DynamicVars["Threshold"].IntValue ? 4 : 2).Execute(ctx);
    }
}