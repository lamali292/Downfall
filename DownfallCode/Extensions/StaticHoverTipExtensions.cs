using BaseLib.Utils;
using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Extensions.Cards
{
    public static class StaticHoverTipCardExtensions
    {
        public static TooltipSource WithVars(this StaticHoverTip staticTip, params DynamicVar[] vars)
        {
            return new TooltipSource(_ => HoverTipFactory.Static(staticTip, vars));
        }
    }
}

namespace Downfall.DownfallCode.Extensions.Relics
{
    public static class StaticHoverTipPowerExtensions
    {
        public static AbstractTooltipSource<RelicModel> WithVars(this StaticHoverTip staticTip,
            params DynamicVar[] vars)
        {
            return new AbstractTooltipSource<RelicModel>(_ => HoverTipFactory.Static(staticTip, vars));
        }
    }
}


namespace Downfall.DownfallCode.Extensions.Powers
{
    public static class StaticHoverTipRelicExtensions
    {
        public static AbstractTooltipSource<PowerModel> WithVars(this StaticHoverTip staticTip,
            params DynamicVar[] vars)
        {
            return new AbstractTooltipSource<PowerModel>(_ => HoverTipFactory.Static(staticTip, vars));
        }
    }
}