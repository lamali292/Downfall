using BaseLib.Utils;
using Champ.ChampCode.Core;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.CustomEnums;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;

namespace Champ.ChampCode.Cards.Rare;

[Pool(typeof(ChampCardPool))]
public class EnchantSword : ChampCardModel
{
    public EnchantSword() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeywords(CardKeyword.Exhaust);
        this.WithEnchantment<Sharp>(8);
        
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override bool ShouldGlowRedInternal => !Owner.Hand.Any(ModelDb.Enchantment<Sharp>().CanEnchant);

    
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var selectorPrefs = new CardSelectorPrefs(DownfallCardSelectorPrefs.ApplySelectionPrompt, 1, 1);
        var card = (await CardSelectCmd.FromHand(ctx, Owner, selectorPrefs, ModelDb.Enchantment<Sharp>().CanEnchant,
            this)).FirstOrDefault();
        if (card == null) return;
        CardCmd.Enchant<Sharp>(card, DynamicVars.Enchantment<Sharp>().BaseValue);
    }
}