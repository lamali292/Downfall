using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;

namespace SlimeBoss.SlimeBossCode.Cards.Common;

[Pool(typeof(SlimeBossCardPool))]
public class Schlurp : SlimeBossCardModel
{
    public Schlurp() : base(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithBlock(6, 2);
        WithTip(SlimeBossTip.Consume);
    }

    protected override Artist Artist => Artist.Get<Freshbone>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var amount = await CommonActions.CardBlock(this, cardPlay);
        await SlimeBossCmd.Consume(ctx, this, cardPlay,
            _ => CommonActions.ApplySelf<BlockNextTurnPower>(ctx, this, amount));
    }
}
