using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.CustomEnums;

namespace Snecko.SneckoCode.Cards.Rare;

[Pool(typeof(SneckoCardPool))]
public class Restock : SneckoCardModel
{
    public Restock() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithCards(6);
        WithTip(SneckoKeywords.Muddle);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var playerState = Owner.PlayerCombatState;
        if (playerState == null) return;
        var cards = playerState.Hand.Cards;
        await CardCmd.DiscardAndDraw(ctx, cards, DynamicVars.Cards.IntValue);
        var newCards = playerState.Hand.Cards;
        await SneckoCmd.Muddle(ctx, newCards, this);
    }
}