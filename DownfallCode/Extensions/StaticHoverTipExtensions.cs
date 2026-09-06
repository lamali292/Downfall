using BaseLib.Utils;
using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Extensions.Cards
{
    public static class StaticHoverTipCardExtensions
    {
        extension(StaticHoverTip staticTip)
        {
            public TooltipSource WithVars(params DynamicVar[] vars)
            {
                return new TooltipSource(_ => HoverTipFactory.Static(staticTip, vars));
            }
        }
    }
}

namespace Downfall.DownfallCode.Extensions.Relics
{
    public static class StaticHoverTipPowerExtensions
    {
        extension(StaticHoverTip staticTip)
        {
            public AbstractTooltipSource<RelicModel> WithVars(params DynamicVar[] vars)
            {
                return new AbstractTooltipSource<RelicModel>(_ => HoverTipFactory.Static(staticTip, vars));
            }
        }
    }
}


namespace Downfall.DownfallCode.Extensions.Powers
{
    public static class StaticHoverTipRelicExtensions
    {
        extension(StaticHoverTip staticTip)
        {
            public AbstractTooltipSource<PowerModel> WithVars(params DynamicVar[] vars)
            {
                return new AbstractTooltipSource<PowerModel>(_ => HoverTipFactory.Static(staticTip, vars));
            }
        }
    }
}

namespace Downfall.DownfallCode.Extensions.Potions
{
    public static class StaticHoverTipCardExtensions
    {
        extension(StaticHoverTip staticTip)
        {
            public AbstractTooltipSource<PotionModel> WithVars(params DynamicVar[] vars)
            {
                return new AbstractTooltipSource<PotionModel>(_ => HoverTipFactory.Static(staticTip, vars));
            }
        }
    }
}
