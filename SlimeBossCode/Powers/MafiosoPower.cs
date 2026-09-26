using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Powers;

// Marker power - the actual "hits ALL enemies" behavior lives in BruiserSlime.Command, which checks for
// this power's presence on the owner.
public class MafiosoPower : SlimeBossPowerModel;
