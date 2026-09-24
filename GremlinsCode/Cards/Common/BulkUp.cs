using BaseLib.Utils;
using Gremlins.GremlinsCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Gremlins.GremlinsCode.Cards.Common;

[Pool(typeof(GremlinsCardPool))]
public class BulkUp : GremlinsCardModel
{
    public BulkUp() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        this.WithTempHp(4, 2);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await GremlinsCmd.GainTempHp(ctx, this);
        await GremlinsCmd.SwapToType<FatGremlin>(ctx, Owner);
    }
}