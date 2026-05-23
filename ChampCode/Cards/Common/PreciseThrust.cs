using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Extensions;
using Champ.ChampCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Cards.Common;

[Pool(typeof(ChampCardPool))]
public class PreciseThrust : ChampCardModel, IBerserkerComboCard, IDefensiveComboCard
{
    public PreciseThrust() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(6, 2);
        WithBlock(6, 2);
    }

    public Task BerserkerComboEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return Task.CompletedTask;
    }

    public async Task DefensiveComboEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        // We have to use `hitCount` here instead of calling `CardAttack` another time
        // in `BerserkerComboEffect`, because calling `CardAttack` twice
        // triggers `VigorPower` for the first time only.
        var count = Owner.ShouldBerserkerComboTrigger() ? 2 : 1;
        await CommonActions.CardAttack(this, cardPlay.Target, count).Execute(ctx);
    }
}