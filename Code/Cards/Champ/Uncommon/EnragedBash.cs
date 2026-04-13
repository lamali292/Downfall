using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Downfall.Code.Cards.Champ.Uncommon;

[Pool(typeof(ChampCardPool))]
public class EnragedBash : ChampCardModel
{
    public EnragedBash() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(7, 3);
        WithVars(new RepeatVar(1));
    }

    protected override bool ShouldGlowGoldInternal => Owner.ShouldBerserkerComboTrigger();

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay, DynamicVars.Repeat.IntValue)
            .Execute(ctx);
        if (!Owner.ShouldBerserkerComboTrigger()) return;
        DynamicVars.Repeat.UpgradeValueBy(1);
    }
}