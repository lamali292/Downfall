using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Extensions;
using Snecko.SneckoCode.Interfaces;

namespace Snecko.SneckoCode.Cards.Common;

[Pool(typeof(SneckoCardPool))]
public class OtherworldlySlash : SneckoCardModel, IHasGift
{
    public OtherworldlySlash() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        this.WithGift(new Gift
        {
            Rarity = CardRarity.Common
        });
        WithDamage(7, 2);
    }


    protected override bool ShouldGlowGoldInternal => PlayedOffClassThisTurn;

    private bool PlayedOffClassThisTurn => CombatManager.Instance.History.CardPlaysFinished.Any(e =>
        e.Actor == Owner.Creature && e.HappenedThisTurn(CombatState) && SneckoCmd.IsOffclass(e.CardPlay.Card));

    public Gift? Gift { get; set; }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (PlayedOffClassThisTurn)
            await CommonActions.CardAttack(this, cardPlay, 2).Execute(ctx);
        else
            await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }
}