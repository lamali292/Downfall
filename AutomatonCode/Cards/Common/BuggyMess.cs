using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Automaton.AutomatonCode.Cards.Common;

[Pool(typeof(AutomatonCardPool))]
public class BuggyMess : AutomatonCardModel, IEncodable
{
    public BuggyMess() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithEnergy(1);
        this.WithTip<Dazed>();
        WithCostUpgradeBy(-1);
    }

    public async Task PlayEncodableEffect(PlayerChoiceContext ctx, CardPlay cardPlay, EncodeContext encodeContext)
    {
        await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
        await DownfallCardCmd.GiveCard<Dazed>(Owner, PileType.Draw);
    }
}