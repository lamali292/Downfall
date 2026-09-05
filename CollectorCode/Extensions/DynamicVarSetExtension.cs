using Collector.CollectorCode.DynamicVars;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Collector.CollectorCode.Extensions;

public static class DynamicVarSetExtension
{
    extension(DynamicVarSet vars)
    {
        public KindleVar Kindle  => (KindleVar) vars._vars["Kindle"];
        public ReserveVar Reserve  => (ReserveVar) vars._vars["Reserve"];
        public CollectorDamageVar CollectorDamage =>  (CollectorDamageVar) vars._vars["CollectorDamage"];
    }
}