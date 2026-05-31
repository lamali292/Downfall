using Champ.ChampCode.Core;
using Champ.ChampCode.CustomEnums;
using Champ.ChampCode.Enchantments;
using Champ.ChampCode.Extensions;
using Champ.ChampCode.Interfaces;
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
            this.WithBerserkerTip();
        }

        if (this is not IDefensiveComboCard) return;
        WithTip(ChampTip.Combo);
        this.WithDefensiveTip();
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
}