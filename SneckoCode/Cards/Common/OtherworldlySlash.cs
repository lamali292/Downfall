using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Common;

[Pool(typeof(SneckoCardPool))]
public class OtherworldlySlash : SneckoCardModel
{
    public OtherworldlySlash() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithGift(new Gift
        {
            Rarity = CardRarity.Common
        });
        WithDamage(7, 2);
    }


    protected override bool ShouldGlowGoldInternal => PlayedOffClassThisTurn;

    private bool PlayedOffClassThisTurn => CombatManager.Instance.History.CardPlaysFinished.Any(e =>
        e.HappenedThisTurn(CombatState) && SneckoCmd.IsOffclass(this, e.CardPlay.Card));

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (PlayedOffClassThisTurn)
            await CommonActions.CardAttack(this, cardPlay, 2).Execute(ctx);
        else
            await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }
}