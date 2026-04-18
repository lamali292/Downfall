using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Cards.Champ.Basic;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Cards.Champ.Rare;

[Pool(typeof(ChampCardPool))]
public class TripleStrike : ChampCardModel
{
    public TripleStrike() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithTags(CardTag.Strike);
        WithDamage(6, 3);
        WithTip(new TooltipSource(StrikeTip));
        WithCards(2);
    }

    private static IHoverTip StrikeTip(CardModel card)
    {
        var strike = ModelDb.GetById<StrikeChamp>(ModelDb.Card<StrikeChamp>().Id).ToMutable();
        if (card.IsUpgraded) strike.UpgradeInternal();
        strike.EnergyCost.SetThisCombat(0);
        strike.AddKeyword(DownfallKeywords.TriggerSkillBonus);
        strike.AddKeyword(CardKeyword.Ethereal);
        strike.AddKeyword(CardKeyword.Exhaust);
        return HoverTipFactory.FromCard(strike);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        if (CombatState == null) return;
        var cardInstances = new List<CardModel>();
        var model = ModelDb.Card<StrikeChamp>();
        for (var i = 0; i < DynamicVars.Cards.IntValue; i++)
        {
            var card = CombatState.CreateCard(model, Owner);
            card.AddKeyword(DownfallKeywords.TriggerSkillBonus);
            card.AddKeyword(CardKeyword.Ethereal);
            card.AddKeyword(CardKeyword.Exhaust);
            if (IsUpgraded) card.UpgradeInternal();
            card.EnergyCost.SetThisCombat(0);
            cardInstances.Add(card);
        }

        await CardPileCmd.AddGeneratedCardsToCombat(cardInstances, PileType.Hand, true);
    }
}