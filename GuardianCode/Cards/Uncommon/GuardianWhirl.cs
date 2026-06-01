using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Guardian.GuardianCode.Cards.Uncommon;

[Pool(typeof(GuardianCardPool))]
public class GuardianWhirl : GuardianCardModel
{
    public GuardianWhirl() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithDamage(4, 2);
        WithVar("Threshold", 16);
        WithTip(StaticHoverTip.Block);
    }

    protected override Artist Artist => Artist.Get<CartesianCanvas>();

    protected override bool ShouldGlowGoldInternal => Owner.Creature.Block >= DynamicVars["Threshold"].IntValue;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions
            .CardAttack(this, cardPlay, Owner.Creature.Block >= DynamicVars["Threshold"].IntValue ? 4 : 2).Execute(ctx);
    }
}