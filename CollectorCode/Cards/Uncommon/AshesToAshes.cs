using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Cards.Uncommon;


[Pool(typeof(CollectorCardPool))]
public class AshesToAshes : CollectorCardModel
{
    public AshesToAshes() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<AshesToAshesPower>(1, 1, false);
        WithTip<Ember>();
        WithCardTip<Ember>(WithPreviewModifiers);
    }
    
    private void WithPreviewModifiers(Ember ember, CardModel cardModel)
    {
        var val = 1;
        if (cardModel.IsUpgraded) val += _currentUpgradeLevel;
        WithModifiers(ember, val);
    }
    
    private static void WithModifiers(Ember ember, int ups)
    {
        DownfallCardCmd.ForceUpgrade(ember, ups);
    }
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.ApplySelf<AshesToAshesPower>(ctx, this);
    }
}