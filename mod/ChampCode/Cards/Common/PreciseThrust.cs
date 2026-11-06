using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Cards.Common;

[Pool(typeof(ChampCardPool))]
public class PreciseThrust : ChampCardModel
{
    public PreciseThrust() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(5, 2);
        WithBlock(5, 2);
        WithBerserkerCombo();
        WithDefensiveCombo();
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        // We have to use `hitCount` here instead of calling `CardAttack` another time
        // under ChampCmd.BerserkerCombo, because calling `CardAttack` twice
        // triggers `VigorPower` for the first time only.
        var count = Owner.ShouldBerserkerComboTrigger ? 2 : 1;
        await CommonActions.CardAttack(this, cardPlay, count).Execute(ctx);
        await ChampCmd.DefensiveCombo(cardPlay, () => CommonActions.CardBlock(this, cardPlay));
    }
}