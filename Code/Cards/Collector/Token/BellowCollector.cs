using BaseLib.Utils;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Core;
using Downfall.Code.Powers.Collector;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Collector.Token;

[Pool(typeof(TokenCardPool))]
public class BellowCollector : CollectorCardModel
{
    public BellowCollector() : base(1, CardType.Skill, CardRarity.Token, TargetType.AnyEnemy)
    {
        WithCostUpgradeBy(-1);
        WithKeywords(CardKeyword.Ethereal, CardKeyword.Exhaust);
        WithCalculatedVar("UnusedBlock", 0, Calc);
        WithTip(typeof(CollectorDoomPower));
        WithTip(StaticHoverTip.Block);
    }

    private static decimal Calc(CardModel card, Creature? creature)
    {
        return DownfallHistory.Get(card.Owner).UnusedBlockLastTurn;
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        var unusedBlock = DownfallHistory.Get(Owner).UnusedBlockLastTurn;
        await CommonActions.Apply<CollectorDoomPower>(cardPlay.Target, this, unusedBlock);
    }


}