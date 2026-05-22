using Automaton.AutomatonCode.Core;
using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace Automaton.AutomatonCode.Cards.Common;

[Pool(typeof(AutomatonCardPool))]
public class Turbo : AutomatonCardModel
{
    public Turbo() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithEnergyTip();
        WithEnergy(2, 1);
        WithTip(typeof(Void));
    }

    protected override async Task PlayEffect(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
        await DownfallCardCmd.GiveCard<Void>(Owner, PileType.Discard);
    }
}