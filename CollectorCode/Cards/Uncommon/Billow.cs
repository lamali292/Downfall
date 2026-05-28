using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class Billow : CollectorCardModel
{
    public Billow() : base(3, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(18, 5);
        this.WithTip<BellowCollector>();
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        var power = await CommonActions.ApplySelf<CopyNextTurnPower>(ctx, this, 1);
        if (power == null) return;
        var card = CombatState!.CreateCard(ModelDb.Card<BellowCollector>(), Owner);
        power.Card = card;
    }
}