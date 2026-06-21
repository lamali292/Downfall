using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Interfaces;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Cards.Uncommon;

[Pool(typeof(ChampCardPool))]
public class FanOfKnives : ChampCardModel, IBerserkerComboCard
{
    public FanOfKnives() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithDamage(4, 2);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    public async Task BerserkerComboEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }
}