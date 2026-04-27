using Downfall.DownfallCode.Powers.Downfall;
using Godot;
using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Guardian.GuardianCode.Gems;

public class TourmalineGem : GemModel
{
    public override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<TemporaryThornsPower>()];

    public override Color GemColor => new(0x06BE7BFF);
    public override CardRarity Rarity => CardRarity.Common;

    public override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var owner = cardPlay.Card.Owner;
        await PowerCmd.Apply<TemporaryThornsPower>(ctx, owner.Creature, 4, owner.Creature, null);
    }
}