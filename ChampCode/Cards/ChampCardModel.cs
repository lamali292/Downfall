using BaseLib.Abstracts;
using Champ.ChampCode.Core;
using Champ.ChampCode.CustomEnums;
using Champ.ChampCode.Enchantments;
using Champ.ChampCode.Events;
using Champ.ChampCode.Extensions;
using Champ.ChampCode.Interfaces;
using Champ.ChampCode.Powers;
using Champ.ChampCode.Stance;
using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Cards;

public abstract class ChampCardModel : DownfallCardModel<Core.Champ>
{
    protected StanceType EnterStance = StanceType.None;

    public ChampCardModel(
        int cost,
        CardType type,
        CardRarity rarity,
        TargetType targetType,
        bool showInCardLibrary = true,
        bool autoAdd = true
    ) : base(cost, type, rarity, targetType, showInCardLibrary, autoAdd)
    {
        if (this is IBerserkerComboCard)
        {
            WithTip(ChampTip.Combo);
            WithBerserkerTip();
        }
        if (this is IDefensiveComboCard)
        {
            WithTip(ChampTip.Combo);
            WithDefensiveTip();
        }
    }
    
    protected void WithDefensiveTip()
    {
        WithTips(e => [ChampModelDb.ChampStance<ChampDefensiveStance>().HoverTip]);
    }

    protected void WithBerserkerTip()
    {
        WithTips(e => [ChampModelDb.ChampStance<ChampBerserkerStance>().HoverTip]);    }

    protected void WithUltimateTip()
    {
        WithTips(e => [ChampModelDb.ChampStance<ChampUltimateStance>().HoverTip]);
    }


    protected override bool ShouldGlowRedInternal =>
        Tags.Contains(ChampTag.Finisher) && Owner.ChampStance().HasFinisher;

    protected override bool ShouldGlowGoldInternal =>
        (this is IBerserkerComboCard && Owner.ShouldBerserkerComboTrigger())
        || (this is IDefensiveComboCard && Owner.ShouldDefensiveComboTrigger());

    protected override bool IsPlayable => !Tags.Contains(ChampTag.Finisher) || Owner.ChampStance().HasFinisher ||
                                          Enchantment is Signature;

    protected virtual async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await Task.CompletedTask;
    }

    protected virtual async Task FinisherEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await ChampCmd.PlayFinisher(ctx, cardPlay);
    }

    protected ConstructedCardModel WithFinisher()
    {
        WithTags(ChampTag.Finisher);
        WithTip(ChampTip.Finisher);
        return this;
    }

    protected ConstructedCardModel WithEnterBerserker()
    {
        EnterStance = StanceType.Berserker;
        WithBerserkerTip();
        return this;
    }

    protected ConstructedCardModel WithEnterDefensive()
    {
        EnterStance = StanceType.Defensive;
        WithDefensiveTip();
        return this;
    }


    protected sealed override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await PlayEffect(ctx, cardPlay);

        var stance = Owner.ChampStance();
        if (Keywords.Contains(ChampKeyword.TriggerSkillBonus)) await stance.SkillBonus(ctx);

        if (cardPlay.Card.Type == CardType.Skill
            && (ChampHook.IgnoreChargeCap(Owner.Creature.CombatState!, Owner) || stance.Charges > 0))
        {
            if (!ChampHook.IgnoreChargeCap(Owner.Creature.CombatState!, Owner))
            {
                stance.Charges--;
                ChampModel.RefreshDisplay(Owner);
            }

            await stance.SkillBonus(ctx);
        }

        if (EnterStance == StanceType.Berserker)
            await ChampCmd.EnterBerserkerStance(ctx, Owner);
        else if (EnterStance == StanceType.Defensive) await ChampCmd.EnterDefensiveStance(ctx, Owner);

        if (this is IBerserkerComboCard berserkerCombo && Owner.ShouldBerserkerComboTrigger())
            await berserkerCombo.BerserkerComboEffect(ctx, cardPlay);
        if (this is IDefensiveComboCard defensiveCombo && Owner.ShouldDefensiveComboTrigger())
            await defensiveCombo.DefensiveComboEffect(ctx, cardPlay);

        if (Tags.Contains(ChampTag.Finisher)) await FinisherEffect(ctx, cardPlay);
    }

    protected ConstructedCardModel WithGlory(int baseVal, int upgrade = 0)
    {
        WithPower<GloryPower>(baseVal, upgrade);
        WithUltimateTip();
        return this;
    }



    protected enum StanceType
    {
        None,
        Berserker,
        Defensive
    }
}