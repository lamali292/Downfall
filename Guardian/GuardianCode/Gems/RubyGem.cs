using Downfall.DownfallCode.Powers.Downfall;
using Godot;
using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Guardian.GuardianCode.Gems;

public class RubyGem : GemModel
{
    public override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<TemporaryStrengthUpPower>()];

    public override Color GemColor => new(0xC52000FF);
    public override CardRarity Rarity => CardRarity.Common;

    public override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var owner = cardPlay.Card.Owner;
        await PowerCmd.Apply<TemporaryStrengthUpPower>(ctx, owner.Creature, 2, owner.Creature, null);
    }
}