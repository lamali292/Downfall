using Collector.CollectorCode.DynamicVars;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Collector.CollectorCode.Extensions;

public static class DynamicVarSetExtension
{
    extension(DynamicVarSet vars)
    {
        public KindleVar Kindle  => (KindleVar) vars["Kindle"];
        public ReserveVar Reserve  => (ReserveVar) vars["Reserve"];
        public TorchheadDamageVar TorchheadDamage =>  (TorchheadDamageVar) vars["TorchheadDamage"];
    }
}