using BaseLib.Utils;
using Champ.ChampCode.Core;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Cards.Uncommon;

[Pool(typeof(ChampCardPool))]
public class EnragedBash : ChampCardModel
{
    public EnragedBash() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(7, 3);
        WithRepeat(1);
        WithBerserkerCombo();
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay, DynamicVars.Repeat.IntValue)
            .Execute(ctx);
        await ChampCmd.BerserkerCombo(cardPlay, () =>
        {
            DynamicVars.Repeat.UpgradeValueBy(1);
            return Task.CompletedTask;
        });
    }
}