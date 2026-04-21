using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Enchantments;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Cards.Champ.Rare;

[Pool(typeof(ChampCardPool))]
public class EnchantShield : ChampCardModel
{
    public EnchantShield() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeywords(CardKeyword.Exhaust);
        WithTip(typeof(Sturdy));
        WithKeyword(CardKeyword.Ethereal, UpgradeType.Remove);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var selectorPrefs = new CardSelectorPrefs(SelectionScreenPrompt, 1, 1);
        var card = (await CardSelectCmd.FromHand(ctx, Owner, selectorPrefs, ModelDb.Enchantment<Sturdy>().CanEnchant,
            this)).FirstOrDefault();
        if (card == null) return;
        CardCmd.Enchant<Sturdy>(card, 1);
    }
}