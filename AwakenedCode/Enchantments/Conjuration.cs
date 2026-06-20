using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.CustomEnums;
using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Awakened.AwakenedCode.Enchantments;

public class Conjuration : DownfallEnchantmentModel<Core.Awakened>
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(AwakenedTip.Conjure)
    ];


    public override bool HasExtraCardText => true;

    public override bool CanEnchant(CardModel card)
    {
        return !card.Tags.Contains(AwakenedTag.Conjure);
    }

    public override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
    {
        return cardPlay?.Card.CombatState != null
            ? AwakenedCmd.Conjure(cardPlay.Card.Owner)
            : Task.CompletedTask;
    }
}