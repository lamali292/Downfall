using BaseLib.Utils;
using Champ.ChampCode.Cards.Basic;
using Champ.ChampCode.Core;
using Champ.ChampCode.CustomEnums;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.CustomEnums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Champ.ChampCode.Cards.Rare;

[Pool(typeof(ChampCardPool))]
public class TripleStrike : ChampCardModel
{
    public TripleStrike() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithTags(CardTag.Strike);
        WithDamage(6, 3);
        WithTip(new TooltipSource(StrikeTip));
        WithTip(DownfallKeyword.Echo);
        WithTip(ChampKeyword.TriggerSkillBonus);
        WithCards(2);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    private static IHoverTip StrikeTip(CardModel card)
    {
        var strike = ModelDb.Card<StrikeChamp>().ToMutable();
        if (card.IsUpgraded) strike.UpgradeInternal();
        strike.EnergyCost.SetThisCombat(0);
        strike.AddKeyword(ChampKeyword.TriggerSkillBonus);
        strike.ToEcho();
        return HoverTipFactory.FromCard(strike);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        if (CombatState == null) return;
        var cardInstances = new List<CardModel>();
        var model = ModelDb.Card<StrikeChamp>();
        for (var i = 0; i < DynamicVars.Cards.IntValue; i++)
        {
            var card = CombatState.CreateCard(model, Owner);
            card.AddKeyword(ChampKeyword.TriggerSkillBonus);
            card.ToEcho();
            if (IsUpgraded) card.UpgradeInternal();
            card.EnergyCost.SetThisCombat(0);
            cardInstances.Add(card);
        }

        await CardPileCmd.AddGeneratedCardsToCombat(cardInstances, PileType.Hand, Owner);
    }
}