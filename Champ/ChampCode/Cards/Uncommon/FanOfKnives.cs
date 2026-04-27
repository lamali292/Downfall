using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Cards.Uncommon;

[Pool(typeof(ChampCardPool))]
public class FanOfKnives : ChampCardModel
{
    public FanOfKnives() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithDamage(4, 2);
    }

    protected override bool ShouldGlowGoldInternal => Owner.ShouldBerserkerComboTrigger();

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var hitCount = Owner.ShouldBerserkerComboTrigger() ? 2 : 1;
        await CommonActions.CardAttack(this, cardPlay, hitCount).Execute(ctx);
    }
}