using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Collector.CollectorCode.DynamicVars;

public class ReserveVar(decimal amount) : DynamicVar("Reserve", amount);