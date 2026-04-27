using Godot;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Guardian.GuardianCode.Gems;

public class SapphireGem : GemModel
{
    public override IEnumerable<IHoverTip> ExtraHoverTips => [GuardianTip.Brace.ToHoverTip()];

    public override Color GemColor => new(0x0624BEFF);
    public override CardRarity Rarity => CardRarity.Common;

    public override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await GuardianCmd.Brace(ctx, cardPlay.Card.Owner, 4);
    }
}