using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Extensions;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Cards.Rare;

[Pool(typeof(ChampCardPool))]
public class SteelEdge : ChampCardModel
{
    public SteelEdge() : base(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(6, 3);
        this.WithFinisher();
    }

    protected override Artist Artist => Artist.Get<CartesianCanvas>();

    protected override bool HasEnergyCostX => true;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var x = ResolveEnergyXValue();
        if (x > 0)
            await CommonActions.CardAttack(this, cardPlay, x).Execute(ctx);
    }

    public override async Task FinisherEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await ChampCmd.PlayFinisher(ctx, cardPlay, repeat: Math.Max(1, ResolveEnergyXValue()));
    }
}