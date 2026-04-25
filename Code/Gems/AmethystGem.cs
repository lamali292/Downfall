using Downfall.Code.Core.Guardian;
using Downfall.Code.Powers.Downfall;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Downfall.Code.Gems;

public class AmethystGem : GemModel
{
    public override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<TemporaryStrengthDownPower>()];

    public override Color GemColor => new(0xA500C9FF);
    public override CardRarity Rarity => CardRarity.Uncommon;

    public override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var owner = cardPlay.Card.Owner;
        if (owner.Creature.CombatState == null) return;
        await PowerCmd.Apply<TemporaryStrengthDownPower>(ctx, owner.Creature.CombatState.Enemies, 2, owner.Creature,
            null);
    }
}