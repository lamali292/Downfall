using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Cards.Collector.Token;
using Downfall.Code.Cards.Gremlins.Token;
using Downfall.Code.Powers.Collector;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Cards.Collector.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class Billow : CollectorCardModel
{
    public Billow() : base(3, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(18, 5);
        WithTip(typeof(BellowCollector));
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        var power = await CommonActions.ApplySelf<CopyNextTurnPower>(this,1);
        if (power == null) return;
        var card = CombatState!.CreateCard(ModelDb.Card<BellowCollector>(), Owner);
        power.Card = card;
    }
}