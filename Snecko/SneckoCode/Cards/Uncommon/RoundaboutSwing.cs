using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Powers;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Uncommon;

[Pool(typeof(SneckoCardPool))]
public class RoundaboutSwing : SneckoCardModel
{
    public RoundaboutSwing() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(8, 3);
        WithPower<DrawCardsNextTurnPower>(2);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        
        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1, 1);
        var newCard = await CardSelectCmd.FromHand(ctx, Owner, prefs, e => e != this, this);
        await CardPileCmd.Add(newCard, PileType.Draw, CardPilePosition.Top);
        await CommonActions.ApplySelf<DrawCardsNextTurnPower>(ctx, this);
    }
}