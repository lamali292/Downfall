using Automaton.AutomatonCode.Cards.Status;
using Automaton.AutomatonCode.Core;
using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Common;

[Pool(typeof(AutomatonCardPool))]
public class Turbo : AutomatonCardModel
{
    public Turbo() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithEnergyTip();
        WithEnergy(2, 1);
        this.WithTip<Error>();
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
        await DownfallCardCmd.GiveCard<Error>(Owner, PileType.Draw);
        await DownfallCardCmd.GiveCard<Error>(Owner, PileType.Discard);
    }
}