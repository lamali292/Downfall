using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.DynamicVars;
using Collector.CollectorCode.Events;
using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Abstract;
using Downfall.DownfallCode.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.ValueProps;

namespace Collector.CollectorCode.Cards;

public abstract class CollectorCardModel
    : DownfallCardModel<Core.Collector>
{
    protected override ICardPlayPhases PlayPhases => CollectorCardPlayPhases.Instance;

    protected CollectorCardModel(
        int cost,
        CardType type,
        CardRarity rarity,
        TargetType targetType,
        bool showInCardLibrary = true,
        bool autoAdd = true) : base(cost, type, rarity, targetType, showInCardLibrary, autoAdd)
    {
        WithTips(e => e.Keywords.Contains(CollectorKeyword.Pyre)
            ?
            [
                HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
            ]
            : []);
    }
    
    
    protected bool ShouldTorcheadTargetAll => _owner != null && CollectorHook.ShouldTorchheadTargetAll(_owner, out _);
    
    /// <summary>
    /// Every Collector card exposes "TorchheadTargetsAll" to its description so Torchhead-attack cards can
    /// switch wording between "lowest-HP enemy" and "ALL enemies". Cards that never reference the arg are unaffected.
    /// </summary>
    protected override void AddExtraArgsToDescription(LocString description)
    {
        description.Add("TorchheadTargetsAll", ShouldTorcheadTargetAll);
        base.AddExtraArgsToDescription(description);
    }

    protected override bool IsPlayable => !IsBlockedByMissingPyreTarget;

    /// <summary>True when this card has Pyre/Megapyre and there's no other hand card to exhaust for it.</summary>
    private bool IsBlockedByMissingPyreTarget => HasPyre && Owner.Hand.All(e => e == this);

    private bool HasPyre => Keywords.Contains(CollectorKeyword.Pyre) || Keywords.Contains(CollectorKeyword.Megapyre);
    
    protected ConstructedCardModel WithKindle(int baseVal, int upgradeVal = 0)
    {
        return WithVar(new KindleVar(baseVal).WithUpgrade(upgradeVal));
    }
    
    protected ConstructedCardModel WithReserve(int baseVal, int upgradeVal = 0)
    {
        WithReserveTip();
        return WithVar(new ReserveVar(baseVal).WithUpgrade(upgradeVal));
    }
    
    protected ConstructedCardModel WithTorchheadDamage(int baseVal, int upgradeVal = 0)
    {
        return WithVar(new TorchheadDamageVar(baseVal, DamageProps.card).WithUpgrade(upgradeVal));
    }



    protected ConstructedCardModel WithReserveTip()
    {
        return WithTip(new TooltipSource(_ => CollectorTip.ReserveTip));
    }

}