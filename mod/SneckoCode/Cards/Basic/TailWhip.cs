using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Snecko.SneckoCode.Cards.Ancient;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Basic;

[Pool(typeof(SneckoCardPool))]
public class TailWhip : SneckoCardModel, ITranscendenceCard
{
    public TailWhip() : base(2, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
        WithOverflow();
        WithDamage(10, 2);
        WithPower<WeakPower>(1, 1);
        WithPower<VulnerablePower>(1, 1);
    }

    public CardModel GetTranscendenceTransformedCard()
    {
        return ModelDb.Card<Whiplash>();
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var overflowing = OverflowCmd.OverflowActive(this);
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await OverflowCmd.Overflow(overflowing, cardPlay, async () =>
        {
            await CommonActions.Apply<WeakPower>(ctx, this, cardPlay);
            await CommonActions.Apply<VulnerablePower>(ctx, this, cardPlay);
        });
    }
}