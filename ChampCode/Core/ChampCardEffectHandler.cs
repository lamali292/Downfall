using Champ.ChampCode.CustomEnums;
using Champ.ChampCode.Events;
using Champ.ChampCode.Extensions;
using Champ.ChampCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Champ.ChampCode.Core;

public static class ChampCardEffectHandler
{
    public static async Task DoAfterOnPlay(CardModel card, PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var owner = card.Owner;
        var stance = owner.ChampStance();


        if (card.Keywords.Contains(ChampKeyword.TriggerSkillBonus))
            await stance.SkillBonus(ctx);

        var ignoreChargeCap = ChampHook.IgnoreChargeCap(owner.Creature.CombatState!, owner);
        if (card.Type == CardType.Skill && (ignoreChargeCap || stance.Charges > 0))
        {
            if (!ignoreChargeCap)
            {
                stance.Charges--;
                ChampModel.RefreshDisplay(owner);
            }

            await stance.SkillBonus(ctx);
        }

        if (card.Tags.Contains(ChampTag.EnterBerserker))
            await ChampCmd.EnterBerserkerStance(ctx, owner);
        else if (card.Tags.Contains(ChampTag.EnterDefensive))
            await ChampCmd.EnterDefensiveStance(ctx, owner);

        if (card is IBerserkerComboCard berserkerCombo && owner.ShouldBerserkerComboTrigger())
            await berserkerCombo.BerserkerComboEffect(ctx, cardPlay);
        if (card is IDefensiveComboCard defensiveCombo && owner.ShouldDefensiveComboTrigger())
            await defensiveCombo.DefensiveComboEffect(ctx, cardPlay);

        if (card.Tags.Contains(ChampTag.Finisher) && card is IFinisherCard finisherCard)
            await finisherCard.FinisherEffect(ctx, cardPlay);
    }
}