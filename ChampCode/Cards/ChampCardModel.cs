using BaseLib.Abstracts;
using Champ.ChampCode.Core;
using Champ.ChampCode.CustomEnums;
using Champ.ChampCode.Enchantments;
using Champ.ChampCode.Extensions;
using Champ.ChampCode.Interfaces;
using Champ.ChampCode.Powers;
using Champ.ChampCode.Stance;
using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Cards;

public abstract class ChampCardModel : DownfallCardModel<Core.Champ>, IFinisherCard
{
    protected ChampCardModel(
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


    protected override bool ShouldGlowRedInternal =>
        Tags.Contains(ChampTag.Finisher) && Owner.ChampStance().HasFinisher;

    protected override bool ShouldGlowGoldInternal =>
        (this is IBerserkerComboCard && Owner.ShouldBerserkerComboTrigger())
        || (this is IDefensiveComboCard && Owner.ShouldDefensiveComboTrigger());

    protected override bool IsPlayable => !Tags.Contains(ChampTag.Finisher) || Owner.ChampStance().HasFinisher ||
                                          Enchantment is Signature;

    public virtual async Task FinisherEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await ChampCmd.PlayFinisher(ctx, cardPlay);
    }

    protected void WithDefensiveTip()
    {
        WithTips(e => [ChampModelDb.ChampStance<ChampDefensiveStance>().HoverTip]);
    }

    protected void WithBerserkerTip()
    {
        WithTips(e => [ChampModelDb.ChampStance<ChampBerserkerStance>().HoverTip]);
    }

    protected void WithUltimateTip()
    {
        WithTips(e => [ChampModelDb.ChampStance<ChampUltimateStance>().HoverTip]);
    }

    protected ConstructedCardModel WithFinisher()
    {
        WithTags(ChampTag.Finisher);
        WithTip(ChampTip.Finisher);
        return this;
    }


    protected ConstructedCardModel WithEnterBerserker()
    {
        WithTags(ChampTag.EnterBerserker);
        WithBerserkerTip();
        return this;
    }

    protected ConstructedCardModel WithEnterDefensive()
    {
        WithTags(ChampTag.EnterDefensive);
        WithDefensiveTip();
        return this;
    }

    protected ConstructedCardModel WithGlory(int baseVal, int upgrade = 0)
    {
        WithPower<GloryPower>(baseVal, upgrade);
        WithUltimateTip();
        return this;
    }
}