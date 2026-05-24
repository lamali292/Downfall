using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
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
        return !AutomatonCmd.IsEncodable(card);
    }

    public override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
    {
        return cardPlay != null
            ? AutomatonCmd.EncodeCard(cardPlay.Card, choiceContext)
            : Task.CompletedTask;
    }
}