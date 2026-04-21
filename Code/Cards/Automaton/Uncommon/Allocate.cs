using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Automaton.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class Allocate : AutomatonCardModel
{
    public Allocate() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithTip(DownfallTip.Status);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var statusCount = PileType.Draw.GetPile(Owner).Cards
            .Count(c => c.Type == CardType.Status);
        if (statusCount > 0)
            await PlayerCmd.GainEnergy(statusCount, Owner);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}