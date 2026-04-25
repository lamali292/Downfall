using Downfall.Code.Core.Guardian;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Gems;

public class OnyxGem : GemModel
{
    public override Color GemColor => new(0x616161FF);
    public override CardRarity Rarity => CardRarity.Rare;

    public override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        foreach (var creaturePower in cardPlay.Card.Owner.Creature.Powers.Where(x => x.Type == PowerType.Debuff))
        {
            var owner = cardPlay.Card.Owner;
            await PowerCmd.ModifyAmount(ctx, creaturePower, -1, owner.Creature, null);
        }
    }
}