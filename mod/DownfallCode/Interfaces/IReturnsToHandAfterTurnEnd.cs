namespace Downfall.DownfallCode.Interfaces;

/// <summary>
/// Marker for a <c>HasTurnEndInHandEffect</c> card that should return to the player's Hand instead of the
/// Discard pile once its <c>OnTurnEndInHand</c> effect finishes. The game hardcodes that discard move
/// (<c>CombatManager.ResolveTurnEndCardEffects</c>), so <see cref="Downfall.DownfallCode.Patches.ReturnToHandAfterTurnEndPatch"/>
/// redirects it for cards implementing this interface.
/// </summary>
public interface IReturnsToHandAfterTurnEnd;
