using Downfall.Code.Core.Hexaghost;
using Downfall.Code.Powers.Hexaghost;
using Downfall.Code.Vfx.Hexaghost;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Ghostflames;

public class BolsteringGhostflame : GhostflameModel
{
    protected override int IgnitionRequirement => 1;
    public override async Task OnIgnite(PlayerChoiceContext ctx)
    {
        var intensity = Owner.Creature.GetPowerAmount<IntensityPower>();
        await CreatureCmd.GainBlock(Owner.Creature,4 + intensity, ValueProp.Move | ValueProp.Unpowered, null);
        await PowerCmd.Apply<StrengthPower>(Owner.Creature, 1, Owner.Creature, null);
    }

    public override NFire.FireColor FireColor => NFire.FireColor.Blue;
    
    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (!IsActive || cardPlay.Card.Owner != Owner || cardPlay.Card.Type != CardType.Power) return;
        if (TryProgress())
            await Ignite(ctx);
       
    }
}
