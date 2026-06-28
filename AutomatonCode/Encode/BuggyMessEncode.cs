using Automaton.AutomatonCode.Core;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Automaton.AutomatonCode.Encode;

public class BuggyMessEncode : EncodeModifier
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new EnergyVar(1)];
    
    public override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (Owner == null) return;
        await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner.Owner);
        await DownfallCardCmd.GiveCard<Dazed>(Owner.Owner, PileType.Draw);
    }
}
