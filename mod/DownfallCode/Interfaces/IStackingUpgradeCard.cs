namespace Downfall.DownfallCode.Interfaces;

/// <summary>
/// A card whose <c>MaxUpgradeLevel</c> grows with its own <c>CurrentUpgradeLevel</c>, allowing it
/// to be upgraded past +1 (e.g. "1 + CurrentUpgradeLevel"). The base game's downgrade
/// (<c>CardModel.DowngradeInternal</c>, used by e.g. the Reflections event) always resets
/// <c>CurrentUpgradeLevel</c> to 0, which is correct for a normal +1 card but wipes every level at
/// once for one of these. <see cref="Downfall.DownfallCode.Patches.StackingUpgradeDowngradePatch"/>
/// re-applies all but one of the removed levels so downgrade only ever removes a single level here too.
/// </summary>
public interface IStackingUpgradeCard;
