using Godot;
using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Gems;

public class CitrineGem : GemModel
{
    public override Color GemColor => new(0xA55B08FF);
    public override CardRarity Rarity => CardRarity.Rare;

    public override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var owner = cardPlay.Card.Owner;
        await PlayerCmd.GainEnergy(1, owner);
    }
}