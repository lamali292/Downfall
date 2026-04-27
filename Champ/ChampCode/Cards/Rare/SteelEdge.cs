using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.CustomEnums;
using Champ.ChampCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Cards.Rare;

[Pool(typeof(ChampCardPool))]
public class SteelEdge : ChampCardModel
{
    public SteelEdge() : base(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(6, 3);
        WithTags(ChampTag.Finisher);
        WithTip(ChampTip.Finisher);
    }

    protected override bool ShouldGlowRedInternal => Owner.ChampStance().HasFinisher;
    protected override bool IsPlayable => Owner.ChampStance().HasFinisher;

    protected override bool HasEnergyCostX => true;

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var x = ResolveEnergyXValue();
        if (x == 0)
        {
            await ChampCmd.PlayFinisher(ctx, cardPlay);
        }
        else
        {
            await CommonActions.CardAttack(this, cardPlay, x).Execute(ctx);
            await ChampCmd.PlayFinisher(ctx, cardPlay, repeat: x);
        }
    }
}