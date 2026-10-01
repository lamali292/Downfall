using Champ.ChampCode.CustomEnums;
using Champ.ChampCode.Extensions;
using Champ.ChampCode.Interfaces;
using Downfall.DownfallCode.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Champ.ChampCode.Core;

/// <summary>Champ's play phases: skill bonus, stance entry, combos and Finisher, in that order after the play.</summary>
public sealed class ChampCardPlayPhases : ICardPlayPhases
{
    public static readonly ChampCardPlayPhases Instance = new();

    public async Task AfterPlay(CardModel card, PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var owner = card.Owner;
        var stance = owner.ChampStance;


        if (card.Keywords.Contains(ChampKeyword.TriggerSkillBonus))
            await stance.SkillBonus(ctx);


        if (card.Tags.Contains(ChampTag.EnterBerserker))
            await ChampCmd.EnterBerserkerStance(ctx, owner);
        else if (card.Tags.Contains(ChampTag.EnterDefensive))
            await ChampCmd.EnterDefensiveStance(ctx, owner);

        if (card is IBerserkerComboCard berserkerCombo && owner.ShouldBerserkerComboTrigger)
            await berserkerCombo.BerserkerComboEffect(ctx, cardPlay);
        if (card is IDefensiveComboCard defensiveCombo && owner.ShouldDefensiveComboTrigger)
            await defensiveCombo.DefensiveComboEffect(ctx, cardPlay);

        if (card.Tags.Contains(ChampTag.Finisher) && card is IFinisherCard finisherCard)
            await ChampCmd.PlayFinisher(ctx, cardPlay, finisherCard.Finisher);
    }
}
