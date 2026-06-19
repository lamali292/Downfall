using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class Repair : AutomatonCardModel
{
    public override bool CanBeGeneratedInCombat => false;
    public Repair() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        this.WithPower<SelfRepairPower>(7, 3, false);
    }

    protected override Artist Artist => Artist.Get<CartesianCanvas>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<SelfRepairPower>(ctx, this);
    }
}