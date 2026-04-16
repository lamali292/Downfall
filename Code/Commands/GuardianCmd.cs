using Downfall.Code.Cards.CardModels;
using Downfall.Code.Cards.Guardian.Abstract;
using Downfall.Code.Core.Guardian;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace Downfall.Code.Commands;

public class GuardianCmd
{
    public static async Task LeaveDefensiveMode(PlayerChoiceContext ctx, Player player)
    {
        await GuardianModel.SetMode<GuardianNormalMode>(ctx, player);
    }

    public static async Task EnterDefensiveMode(PlayerChoiceContext ctx, Player player)
    {
        await GuardianModel.SetMode<GuardianDefensiveMode>(ctx, player);
    }
    
    public static async Task ChangeMode(PlayerChoiceContext ctx, Player player)
    {
        if (GuardianModel.IsInMode<GuardianNormalMode>(player))
            await EnterDefensiveMode(ctx, player);   
        else
            await LeaveDefensiveMode(ctx, player);
    }



    public static async Task PutGemIn(CardModel gem, CardModel card)
    {
        if (card is not GuardianCardModel guardianCard) return;
        if (gem is not IGemCard gemCard) return;
        if (!guardianCard.CanAddGem(gemCard.GemModel)) return;
        
        guardianCard.AddGem(gemCard.GemModel);
        await CardPileCmd.RemoveFromDeck(gem, false);
        await Cmd.Wait(0.5f);
        if (LocalContext.IsMe(card.Owner))
            NRun.Instance?.GlobalUi.CardPreviewContainer.AddChildSafely(NCardSmithVfx.Create([
                    card
                ])!);
        await Cmd.Wait(0.5f);
    }
    
}