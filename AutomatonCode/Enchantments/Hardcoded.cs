using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Enchantments;

public class Hardcoded : AutomatonEnchantmentModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(AutomatonTip.Encode)
    ];


    public override bool HasExtraCardText => false;

    public override bool CanEnchant(CardModel card)
    {
        return base.CanEnchant(card) && AutomatonCmd.IsEncodable(card);
    }
    
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (player !=  Card.Owner || Card.Owner.PlayerCombatState is not { TurnNumber: 1 }) return;
        await AutomatonCmd.EncodeCard(Card, ctx);
    }

    
}