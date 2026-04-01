using Downfall.Code.Abstract;
using Downfall.Code.Displays;
using Downfall.Code.Events;
using Downfall.Code.Interfaces;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace Downfall.Code.Powers.Automaton;

public class OptimizePower : AutomatonPowerModel, IOnEncode
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task OnCardEncoded(PlayerChoiceContext ctx, CardModel encodedCard, CardPlay cardPlay)
    {
        if (encodedCard.Owner != Owner.Player) return;
        if (encodedCard.IsUpgraded) return;
        if (Amount <= 0) return;
        await PowerCmd.Decrement(this);
        encodedCard.UpgradeInternal();
        if (LocalContext.IsMe(encodedCard.Owner))
        {
            var cardNode = NCard.FindOnTable(encodedCard);
            if (cardNode != null)
            {
                cardNode.UpdateVisuals(encodedCard.Pile?.Type ?? PileType.Play, CardPreviewMode.Normal);
                var vfx = NCardSmithVfx.Create(cardNode);
                if (vfx != null) NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(vfx);
            }

            AutomatonDisplay.Refresh(encodedCard.Owner);
        }
    }
}