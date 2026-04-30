using Godot;
using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Gems;

public class OnyxGem : GemModel
{
    public override Color GemColor => new(0x616161FF);
    public override CardRarity Rarity => CardRarity.Rare;

    public override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await GuardianCmd.Polish(ctx, cardPlay.Card, 1);
    }
}