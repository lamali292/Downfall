using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Extensions;
using Snecko.SneckoCode.Interfaces;

namespace Snecko.SneckoCode.Cards.Uncommon;

[Pool(typeof(SneckoCardPool))]
public class ComboString : SneckoCardModel, IHasGift
{
    public ComboString() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        this.WithGift(new Gift
        {
            Rarity = CardRarity.Uncommon
        });
        WithDamage(7, 2);
        WithCalculatedVar("Repeat", 0, CalcDamage);
    }

    public Gift? Gift { get; set; }

    private static decimal CalcDamage(CardModel card, Creature? _)
    {
        return CombatManager.Instance.History
            .CardPlaysFinished.Count(e =>
                e.HappenedThisTurn(card.CombatState) &&
                SneckoCmd.IsOffclass(card, e.CardPlay.Card));
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var repeat = (int)DynamicVars["Repeat"].Calculate(null);
        await CommonActions.CardAttack(this, cardPlay, repeat).Execute(ctx);
    }
}