using BaseLib.Abstracts;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.CustomEnums;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Enchantments;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class Repurpose : SlimeBossCardModel
{
    public Repurpose() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithEnchantment<Swift>(1, 1);
        WithCardTip<Slimed>(EnchantSlimed);
        WithCards(2);
    }

    private static void EnchantSlimed(Slimed slimed, CardModel card)
    {
        DownfallCardCmd.Enchant<Swift>(slimed, card.DynamicVars.Enchantment<Swift>().BaseValue);
    }
    protected override Artist Artist => Artist.Get<Thelethargicweirdo>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var prefs = new CardSelectorPrefs(DownfallCardSelectorPrefs.ToTopSelectionPrompt, DynamicVars.Cards.IntValue);
        var cards = await CardSelectCmd.FromCombatPile(ctx, PileType.Draw.GetPile(Owner), Owner, prefs);
        foreach (var card in cards)
        {
            var slimed = CombatState?.CreateCard<Slimed>(Owner);
            if (slimed == null) continue;
            EnchantSlimed(slimed, this);
            await CardCmd.Transform(card, slimed);
        }
    }
}
