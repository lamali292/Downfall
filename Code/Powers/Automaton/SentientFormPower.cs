using Downfall.Code.Abstract;
using Downfall.Code.Cards.Automaton.Token;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace Downfall.Code.Powers.Automaton;

public class SentientFormPower : AutomatonPowerModel
{
    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Owner.Creature != Owner || card is not FunctionCard functionCard) return;
        await Cmd.Wait(0.25f);
        foreach (var varSet in functionCard.getDynamicVars())
            foreach (var dynVar in varSet.Values)
                dynVar.UpgradeValueBy(Amount);

        if (functionCard.BaseReplayCount > 0) functionCard.BaseReplayCount += Amount;
        // maybe better animation?
        if (!LocalContext.IsMe(functionCard.Owner)) return;
        var cardNode = NCard.FindOnTable(functionCard);
        if (cardNode == null) return;
        cardNode.UpdateVisuals(functionCard.Pile?.Type ?? PileType.Play, CardPreviewMode.Normal);
        var vfx = NCardSmithVfx.Create(cardNode);
        if (vfx != null) NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(vfx);
    }
}