using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Cards.Guardian.Abstract;
using Downfall.Code.Core.Guardian;
using Downfall.Code.Extensions;
using Downfall.Code.Powers.Guardian;
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
    public static async Task LeaveDefensiveMode(Player player)
    {
        await GuardianModel.SetMode<GuardianNormalMode>(player);
    }

    public static async Task EnterDefensiveMode(Player player)
    {
        await GuardianModel.SetMode<GuardianDefensiveMode>(player);
    }
    
    public static async Task ChangeMode(Player player)
    {
        if (GuardianModel.IsInMode<GuardianNormalMode>(player))
            await EnterDefensiveMode(player);   
        else
            await LeaveDefensiveMode(player);
    }

    public static async Task Brace(Player player, int amount)
    {
        var power = player.Creature.GetPower<ModeShiftPower>();
        if (power == null) return;
        var a = await PowerCmd.ModifyAmount(power, -amount, player.Creature, null);
        if (a > 0) return;
        await power.Reset();
    }
    
    public static async Task Brace(CardModel card)
    {
        await Brace(card.Owner, card.DynamicVars.Brace().IntValue);
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