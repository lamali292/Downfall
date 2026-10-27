using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Hexaghost.HexaghostCode.Powers;

public class PremonitionPower : HexaghostPowerModel
{
    public PremonitionPower()
    {
        WithTip(CardKeyword.Ethereal);
        WithTip(CardKeyword.Exhaust);
        WithTip<StrengthPower>();
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card,
        bool causedByEthereal)
    {
        if (card.Owner.Creature != Owner || !card.Keywords.Contains(CardKeyword.Ethereal)) return;
        Flash();
        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, Amount, Owner, null);
    }
}
