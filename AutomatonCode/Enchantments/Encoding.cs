using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Enchantments;

public class Encoding : AutomatonEnchantmentModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(AutomatonTip.Encode)
    ];


    public override bool HasExtraCardText => true;

    public override bool CanEnchant(CardModel card)
    {
        if (!base.CanEnchant(card)) 
            return false;
        return !AutomatonCmd.IsEncodable(card);
    }

    public override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
    {
        return cardPlay != null
            ? AutomatonCmd.EncodeCard(cardPlay.Card, choiceContext)
            : Task.CompletedTask;
    }
}