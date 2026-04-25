using Downfall.Code.Core.Guardian;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Gems;

public class GarnetGem : GemModel
{
    public override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<VulnerablePower>()];

    public override Color GemColor => new(0x5D0109FF);
    public override CardRarity Rarity => CardRarity.Uncommon;

    public override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var owner = cardPlay.Card.Owner;
        if (owner.Creature.CombatState == null) return;
        await PowerCmd.Apply<VulnerablePower>(ctx, owner.Creature.CombatState.Enemies, 1, owner.Creature, null);
    }
}