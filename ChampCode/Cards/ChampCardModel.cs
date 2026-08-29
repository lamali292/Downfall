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
        if (this is IBerserkerComboCard) WithTip(ChampTip.Combo);

        if (this is not IDefensiveComboCard) return;
        WithTip(ChampTip.Combo);
    }


    protected override bool ShouldGlowRedInternal =>
        Tags.Contains(ChampTag.Finisher) && Owner.ChampStance.HasFinisher;

    protected override bool ShouldGlowGoldInternal =>
        (this is IBerserkerComboCard && Owner.ShouldBerserkerComboTrigger)
        || (this is IDefensiveComboCard && Owner.ShouldDefensiveComboTrigger);

    protected override bool IsPlayable => !Tags.Contains(ChampTag.Finisher) || Owner.ChampStance.HasFinisher ||
                                          Enchantment is Signature;

    public virtual async Task FinisherEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await ChampCmd.PlayFinisher(ctx, cardPlay);
    }

    public virtual bool AffectsAllPlayers => false;


    public ConstructedCardModel WithDefensiveTip()
    {
        return WithTips(e => ChampModelDb.ChampStance<ChampDefensiveStance>().HoverTips);
    }

    public ConstructedCardModel WithBerserkerTip()
    {
        return WithTips(e => ChampModelDb.ChampStance<ChampBerserkerStance>().HoverTips);
    }

    public ConstructedCardModel WithUltimateTip()
    {
        return WithTips(e => ChampModelDb.ChampStance<ChampUltimateStance>().HoverTips);
    }

    public ConstructedCardModel WithFinisher()
    {
        WithTags(ChampTag.Finisher);
        WithTip(ChampTip.Finisher);
        return this;
    }


    public ConstructedCardModel WithEnterBerserker()
    {
        WithTags(ChampTag.EnterBerserker);
        WithBerserkerTip();
        return this;
    }

    public ConstructedCardModel WithEnterDefensive()
    {
        WithTags(ChampTag.EnterDefensive);
        WithDefensiveTip();
        return this;
    }

    public ConstructedCardModel WithGlory(int baseVal, int upgrade = 0)
    {
        WithPower<GloryPower>(baseVal, upgrade);
        //card.WithUltimateTip();
        return this;
    }
}