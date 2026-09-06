using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Encounters;

namespace Collector.CollectorCode.Cards.Collectibles;

public class KaiserCrabCard : Collectible<KaiserCrabBoss>
{
    public KaiserCrabCard() : base(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, 0.3f)
    {
        WithDamage(14, 1);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        var card = RunState!.Rng.CombatCardSelection.NextItem(Owner.Hand.Where(e => e.Type == CardType.Attack));
        if (card != null) await CardCmd.AutoPlay(ctx, card, null);
    }
}