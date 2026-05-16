using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Champ.ChampCode.Cards.Uncommon;

[Pool(typeof(ChampCardPool))]
public class GoodCleanFight : ChampCardModel
{
    public GoodCleanFight() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithPower<StrengthPower>(2, 1);
        WithPower<DexterityPower>(2, 1);
    }

    protected override bool ShouldGlowGoldInternal =>
        Owner.ShouldBerserkerComboTrigger() || Owner.ShouldDefensiveComboTrigger();

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (Owner.ShouldBerserkerComboTrigger()) await CommonActions.ApplySelf<StrengthPower>(ctx, this);
        if (Owner.ShouldDefensiveComboTrigger()) await CommonActions.ApplySelf<DexterityPower>(ctx, this);
    }
}