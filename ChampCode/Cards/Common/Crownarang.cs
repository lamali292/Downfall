using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Interfaces;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Cards.Common;

[Pool(typeof(ChampCardPool))]
public class Crownarang : ChampCardModel, IBerserkerComboCard
{
    public Crownarang() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(8, 2);
        WithCards(2, 1);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    public async Task BerserkerComboEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Draw(this, ctx);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }
}