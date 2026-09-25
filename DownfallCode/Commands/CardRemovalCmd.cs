using BaseLib.Commands;
using Downfall.DownfallCode.Compatibility;
using Downfall.DownfallCode.Utils;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.TestSupport;

namespace Downfall.DownfallCode.Commands;

/// <summary>
///     Destroy/removal visuals for a card leaving combat. Split out of <see cref="DownfallCardCmd" />,
///     which owns card acquisition instead.
/// </summary>
public class CardRemovalCmd
{
    public static async Task RemoveFromCombat(IEnumerable<CardModel> cards)
    {
        var list = cards.ToList();
        foreach (var card in list)
        {
            await PlayDestroyPreview(card, 0.1f);
        }
        await CardPileCmd.RemoveFromCombat(list, true);
    }

    private static async Task PlayDestroyPreview(CardModel card, float delay)
    {
        if (!LocalContext.IsMine(card)) return;

        if (delay > 0f)
            await Cmd.Wait(delay);

        var cardNode = NCard.Create(card);
        if (cardNode == null) return;

        var room = NCombatRoom.Instance;
        if (room == null) return;

        var container = room.Ui.MessyCardPreviewContainer;
        container.AddChildSafely(cardNode);
        cardNode.UpdateVisuals(PileType.None, CardPreviewMode.Normal);

        var tween = cardNode.CreateTween();
        tween.TweenProperty(cardNode, "scale", Vector2.One, 0.25)
            .From(Vector2.Zero)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Cubic);

        if (!TestMode.IsOn && CardRemoveVfxCompat.IsAvailable)
        {
            tween.TweenInterval(0.25);
            tween.TweenCallback(Callable.From(() =>
            {
                var vfx = CardRemoveVfxCompat.Create(cardNode);
                if (vfx != null)
                    container.AddChildSafely(vfx);
            }));
            tween.TweenInterval(CardRemoveVfxCompat.DeleteCardDelay);
        }
        tween.TweenCallback(Callable.From(cardNode.QueueFreeSafely));
    }
}
