using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Gremlins.GremlinsCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Gremlins.GremlinsCode.Cards.Uncommon;

[Pool(typeof(GremlinsCardPool))]
public class EdibleArmor : GremlinsCardModel
{
    public EdibleArmor() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithTip(StaticHoverTip.Block);
        WithKeyword(CardKeyword.Exhaust);
        WithCostUpgradeBy(-1);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var block = Owner.Creature.Block;
        await CreatureCmd.LoseBlock(ctx, Owner.Creature, block, Owner.Creature);
        await DownfallCmd.GainTempHp(ctx, this, block);
    }
}