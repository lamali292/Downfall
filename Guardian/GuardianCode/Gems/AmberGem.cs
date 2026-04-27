using Godot;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Guardian.GuardianCode.Gems;

public class AmberGem : GemModel
{
    public override IEnumerable<IHoverTip> ExtraHoverTips => [GuardianTip.Accelerate.ToHoverTip()];

    public override Color GemColor => new(0xD0D100FF);
    public override CardRarity Rarity => CardRarity.Uncommon;

    public override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await GuardianCmd.Accelerate(ctx, cardPlay.Card.Owner);
    }
}