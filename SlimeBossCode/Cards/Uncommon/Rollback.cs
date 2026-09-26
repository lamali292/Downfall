using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Enchantments;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class Rollback : SlimeBossCardModel
{
    public Rollback() : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(3, 1);
        WithEnchantment<Adroit>(8, 3);
        WithCardTip<Slimed>(EnchantSlimed);

    }

    private static void EnchantSlimed(Slimed slimed, CardModel card)
    {
        DownfallCardCmd.Enchant<Adroit>(slimed, card.DynamicVars.Enchantment<Adroit>().BaseValue);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await DownfallCardCmd.GiveCard<Slimed>(Owner, PileType.Draw, CardPilePosition.Top,
            action: card => EnchantSlimed(card, this));
    }
}