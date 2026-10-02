using BaseLib.Abstracts;
using Champ.ChampCode.Core;
using Champ.ChampCode.CustomEnums;
using Champ.ChampCode.Extensions;
using Champ.ChampCode.Events;
using Champ.ChampCode.Interfaces;
using Champ.ChampCode.Powers;
using Champ.ChampCode.Stance;
using Downfall.DownfallCode.Abstract;
using Downfall.DownfallCode.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

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
        WithTips(card => card.Tags.Contains(ChampTag.BerserkerCombo) || card.Tags.Contains(ChampTag.DefensiveCombo)
            ? [HoverTipFactory.Static(ChampTip.Combo)]
            : []);
    }


    protected override bool ShouldGlowRedInternal =>
        ChampCmd.FinisherCanAct(this);

    protected override bool ShouldGlowGoldInternal =>
        (Tags.Contains(ChampTag.BerserkerCombo) && Owner.ShouldBerserkerComboTrigger)
        || (Tags.Contains(ChampTag.DefensiveCombo) && Owner.ShouldDefensiveComboTrigger);

    protected override bool IsPlayable => !Tags.Contains(ChampTag.Finisher) || ChampCmd.FinisherCanAct(this);

    public virtual FinisherDescriptor Finisher => FinisherDescriptor.Default;


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


    /// <summary>Marks the card as a Berserker combo card for the glow/tip; the card's own OnPlayInternal
    /// still has to call <see cref="ChampCmd.BerserkerCombo"/> to actually run the combo effect.</summary>
    public ConstructedCardModel WithBerserkerCombo()
    {
        WithTags(ChampTag.BerserkerCombo);
        return this;
    }

    /// <summary>Marks the card as a Defensive combo card for the glow/tip; the card's own OnPlayInternal
    /// still has to call <see cref="ChampCmd.DefensiveCombo"/> to actually run the combo effect.</summary>
    public ConstructedCardModel WithDefensiveCombo()
    {
        WithTags(ChampTag.DefensiveCombo);
        return this;
    }

    public ConstructedCardModel WithGlory(int baseVal, int upgrade = 0)
    {
        WithPower<GloryPower>(baseVal, upgrade);
        //card.WithUltimateTip();
        return this;
    }
}