using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Downfall.DownfallCode.Extensions;

public static class DynamicVarExtension
{
    extension(DynamicVar var)
    {
        public decimal Calculate(Creature? target)
        {
            return var is CalculatedVar calculatedVar ? calculatedVar.Calculate(target) : 0;
        }


    }
    
    extension<T>(T source) where T : DynamicVar
    {
        public T WithMyTooltip(
            string? locKey = null,
            string locTable = "static_hover_tips")
        {
            var key = locKey ?? source.GetType().GetPrefix() + StringHelper.Slugify(source.Name);
            DynamicVarExtensions.DynamicVarTips[source] = (Func<DynamicVar, IHoverTip>)(locVar =>
            {
                var title = new LocString(locTable,
                    LocString.Exists(locTable, key + ".smartTitle")
                        ? key + ".smartTitle"
                        : key + ".title");
                var description = new LocString(locTable,
                    LocString.Exists(locTable, key + ".smartDescription")
                        ? key + ".smartDescription"
                        : key + ".description");
                title.Add(locVar);
                description.Add(locVar);
                return (IHoverTip)new HoverTip(title, description);
            });
            return source;
        }
    }
}