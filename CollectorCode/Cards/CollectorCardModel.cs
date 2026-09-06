using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.DynamicVars;
using Collector.CollectorCode.Interfaces;
using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace Collector.CollectorCode.Cards;

public abstract class CollectorCardModel
    : DownfallCardModel<Core.Collector>
{
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
    
    
    protected override bool IsPlayable =>
        !HasPyre|| (HasPyre && Owner.Hand.Any(e => e != this));

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
    
    protected ConstructedCardModel WithCollectorDamage(int baseVal, int upgradeVal = 0)
    {
        return WithVar(new CollectorDamageVar(baseVal, DamageProps.card).WithUpgrade(upgradeVal));
    }



    protected ConstructedCardModel WithReserveTip()
    {
        return WithTip(new TooltipSource(_ => CollectorTip.ReserveTip));
    }

}