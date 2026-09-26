using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;
using SlimeBoss.SlimeBossCode.Interfaces;

namespace SlimeBoss.SlimeBossCode.Cards.Common;

[Pool(typeof(SlimeBossCardPool))]
public class Schlurp : SlimeBossCardModel, IHasConsumeEffect
{
    private decimal? _amount;

    public Schlurp() : base(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithBlock(5, 2);
        WithTip(SlimeBossTip.Consume);
    }

    protected override Artist Artist => Artist.Get<Freshbone>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var amount = await CommonActions.CardBlock(this, cardPlay);
        // TODO - look how to make this better
        _amount = amount;
        await SlimeBossCmd.Consume(ctx, this, cardPlay);
        _amount = null;
    }

    public async Task ConsumeEffect(PlayerChoiceContext ctx, CardPlay? cardPlay, Creature target)
    {
        if (!_amount.HasValue) return;
        await CommonActions.ApplySelf<BlockNextTurnPower>(ctx, this, _amount.Value);
    }
}
