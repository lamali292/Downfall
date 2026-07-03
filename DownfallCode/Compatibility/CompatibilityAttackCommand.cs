using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Compatibility;

public static class CompatibilityAttackCommand
{
    public static AttackCommand DownfallFromCard(this AttackCommand command, CardModel card, CardPlay? cardPlay)
    {
#if V107
        return command.FromCard(card);
#else
        return command.FromCard(card, cardPlay);
#endif
    }
}