using MegaCrit.Sts2.Core.Commands;
using Collector.CollectorCode.Powers;
using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Cards.Rare;

[Pool(typeof(CollectorCardPool))]
public class Darkstorm : CollectorCardModel
{
    public Darkstorm() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithPower<DarkstormPower>(1, false);
        WithCardTip<Ember>();
        WithCostUpgradeBy(-1);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.ApplySelf<DarkstormPower>(ctx, this);
    }
}
