using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.CustomEnums;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Powers;

public class GiftsFromBeyondPower : HexaghostPowerModel
{
    public GiftsFromBeyondPower()
    {
        WithTip(HexaghostKeyword.Afterlife);
    }
    
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        if (player.Creature != Owner) return;
        var cards = HexaghostCmd.GetAfterlifeCards(player, Amount);
        await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Hand, player);
    }
}