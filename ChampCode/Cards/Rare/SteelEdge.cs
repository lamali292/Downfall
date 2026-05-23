using BaseLib.Utils;
using Champ.ChampCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Cards.Rare;

[Pool(typeof(ChampCardPool))]
public class SteelEdge : ChampCardModel
{
    public SteelEdge() : base(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(6, 3);
        WithFinisher();
    }

    protected override bool HasEnergyCostX => true;

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var x = ResolveEnergyXValue();
        if (x > 0)
            await CommonActions.CardAttack(this, cardPlay, x).Execute(ctx);
    }

    protected override async Task FinisherEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await ChampCmd.PlayFinisher(ctx, cardPlay, repeat: Math.Max(1, ResolveEnergyXValue()));
    }
}