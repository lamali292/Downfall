using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.CustomEnums;
using Downfall.DownfallCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Afflictions;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Cards.Collectibles;

public class QueenCard : Collectible<QueenBoss>
{
    public QueenCard() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self, 0.17f)
    {
        WithKeyword(CollectorKeyword.Megapyre);
        WithTip(CollectorTip.Pyred);
        WithKeyword(CardKeyword.Exhaust);
        WithCostUpgradeBy(-1);
        WithEnergy(3);
        WithPower<ChainsOfBindingPower>(3, -1, false);
        WithTips(_ => HoverTipFactory.FromAffliction<Bound>());
    }
    
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<PyrePower>(ctx, this, DynamicVars.Energy.BaseValue);
        //I use pyre power because it's funny that the pyre card gives you the pyre power on the pyre character.
        await CommonActions.ApplySelf<ChainsOfBindingPower>(ctx, this);
    }
}
