using Downfall.Code.Core.Guardian;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Gems;

public class OpalGem : GemModel
{
    public override Color GemColor => new(0xC7C7C7FF);
    public override CardRarity Rarity => CardRarity.Uncommon;

    public override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var owner = cardPlay.Card.Owner;
        await CardPileCmd.Draw(ctx, owner);
    }
}