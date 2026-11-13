using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class InfiniteBeams : AutomatonCardModel
{
    public InfiniteBeams() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithTip<MinorBeam>();
        WithCostUpgradeBy(-1);
        WithPower<InfiniteBeamsPower>(1, false);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.ApplySelf<InfiniteBeamsPower>(ctx, this);
    }
}