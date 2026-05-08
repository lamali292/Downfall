using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Powers;

public class EntangledPower() : DownfallPowerModel(PowerType.Debuff, PowerStackType.Single)
{
    public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)
    {
        return card.Owner != Owner.Player || card.Type != CardType.Attack;
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        await PowerCmd.Remove(this);
    }
}