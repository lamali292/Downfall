using BaseLib.Utils;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.History;
using Downfall.Code.Powers.Collector;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

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
        return CombatManager.Instance.History.Entries.OfType<UnusedBlockEntry>().Where(x =>
            x.Actor == card.Owner.Creature && x.HappenedThisTurn(card.Owner.Creature.CombatState)
        ).Sum(x => x.Amount);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        var unusedBlock = Calc(this, cardPlay.Target);
        await CommonActions.Apply<CollectorDoomPower>(ctx, cardPlay.Target, this, unusedBlock);
    }
}