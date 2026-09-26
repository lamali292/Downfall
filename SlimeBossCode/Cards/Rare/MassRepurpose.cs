using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.CustomEnums;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Enchantments;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Cards.Rare;

[Pool(typeof(SlimeBossCardPool))]
public class MassRepurpose : SlimeBossCardModel
{
    public MassRepurpose() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithEnchantment<Adroit>(5, 2);
        WithCardTip<Slimed>(EnchantSlimed);
    }

    private static void EnchantSlimed(Slimed slimed, CardModel card)
    {
        DownfallCardCmd.Enchant<Swift>(slimed, card.DynamicVars.Enchantment<Adroit>().BaseValue);
    }
    
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var cards = await DownfallCardSelectionCmd.SelectFromHand(ctx,
            CardSelectorPrefs.TransformSelectionPrompt, Owner.Hand.Count, this, c => c != this, true);
        foreach (var card in cards)
        {
            var slimed = CombatState?.CreateCard<Slimed>( Owner);
            if (slimed == null) continue;
            EnchantSlimed(slimed, this);
            await CardCmd.Transform(card, slimed);
        }
    }
}
