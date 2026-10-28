using MegaCrit.Sts2.Core.Commands;
using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

using Collector.CollectorCode.Cards.Token;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Models;
namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class Karma : CollectorCardModel
{
    public Karma() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<KarmaPower>(1, false);
        WithCardTip<Ember>();
        WithKeyword(CardKeyword.Innate, UpgradeType.Add);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.ApplySelf<KarmaPower>(ctx, this);
    }
}
